import type { AiAttachmentRequest } from "./types";

export const MAX_AI_ATTACHMENTS = 4;
export const MAX_AI_IMAGE_BYTES = 5 * 1024 * 1024;
export const MAX_AI_TEXT_BYTES = 256 * 1024;
export const AI_ATTACHMENT_ACCEPT = [
  "image/png",
  "image/jpeg",
  "image/webp",
  "image/gif",
  ".txt",
  ".md",
  ".json",
  ".jsonl",
  ".xml",
  ".yaml",
  ".yml",
  ".csv",
  ".log",
  ".cs",
  ".csproj",
  ".sln",
  ".props",
  ".targets",
  ".ts",
  ".tsx",
  ".js",
  ".jsx",
  ".mjs",
  ".cjs",
  ".css",
  ".scss",
  ".html",
  ".sql",
  ".py",
  ".java",
  ".kt",
  ".go",
  ".rs",
  ".rb",
  ".php",
  ".swift",
  ".sh",
  ".zsh",
  ".env",
  "Dockerfile",
].join(",");

const imageTypes = new Set(["image/png", "image/jpeg", "image/webp", "image/gif"]);
const textExtensions = new Set(
  AI_ATTACHMENT_ACCEPT.split(",").filter((value) => value.startsWith(".")),
);

export interface PendingAiAttachment extends AiAttachmentRequest {
  id: string;
  size: number;
  previewUrl?: string;
}

export async function readAiAttachment(file: File): Promise<PendingAiAttachment> {
  const mediaType = file.type.toLowerCase();
  const extension = file.name.includes(".")
    ? `.${file.name.split(".").pop()?.toLowerCase()}`
    : "";
  const isImage = imageTypes.has(mediaType);

  if (isImage) {
    if (file.size > MAX_AI_IMAGE_BYTES)
      throw new Error(`${file.name} is larger than 5 MB.`);

    const previewUrl = await readAsDataUrl(file);
    return {
      id: crypto.randomUUID(),
      fileName: file.name,
      mediaType,
      content: previewUrl.slice(previewUrl.indexOf(",") + 1),
      isImage: true,
      size: file.size,
      previewUrl,
    };
  }

  const isText =
    mediaType.startsWith("text/") ||
    ["application/json", "application/xml", "application/yaml", "application/x-yaml", "application/javascript"].includes(mediaType) ||
    textExtensions.has(extension) ||
    file.name.toLowerCase() === "dockerfile";
  if (!isText)
    throw new Error(`${file.name} is not a supported text, code, or image file.`);
  if (file.size > MAX_AI_TEXT_BYTES)
    throw new Error(`${file.name} is larger than 256 KB.`);

  const content = await file.text();
  if (!content.trim() || content.includes("\u0000"))
    throw new Error(`${file.name} is empty or is not a plain-text file.`);

  return {
    id: crypto.randomUUID(),
    fileName: file.name,
    mediaType: mediaType || "text/plain",
    content,
    isImage: false,
    size: file.size,
  };
}

export function extractCodeSuggestion(content: string): string | undefined {
  const blocks: string[] = [];
  const expression = /```[^\n`]*\n([\s\S]*?)```/g;
  for (const match of content.matchAll(expression)) {
    const code = match[1].trim();
    if (code) blocks.push(code);
  }

  return blocks.sort((left, right) => right.length - left.length)[0];
}

export function formatAttachmentSize(size: number): string {
  if (size < 1024) return `${size} B`;
  if (size < 1024 * 1024) return `${Math.ceil(size / 1024)} KB`;
  return `${(size / 1024 / 1024).toFixed(1)} MB`;
}

function readAsDataUrl(file: File): Promise<string> {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.onload = () => resolve(String(reader.result));
    reader.onerror = () => reject(new Error(`Could not read ${file.name}.`));
    reader.readAsDataURL(file);
  });
}
