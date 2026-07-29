import { describe, expect, it } from "vitest";
import { extractCodeSuggestion, formatAttachmentSize, readAiAttachment } from "./attachmentUtils";

describe("AI attachment utilities", () => {
  it("extracts the largest fenced code block without Markdown", () => {
    const response = [
      "Use this implementation:",
      "```ts",
      "const small = true;",
      "```",
      "Then replace the file with:",
      "```csharp",
      "public sealed class Example",
      "{",
      "    public int Value { get; init; }",
      "}",
      "```",
    ].join("\n");

    expect(extractCodeSuggestion(response)).toBe([
      "public sealed class Example",
      "{",
      "    public int Value { get; init; }",
      "}",
    ].join("\n"));
  });

  it("does not make prose applyable as source code", () => {
    expect(extractCodeSuggestion("Explain the behavior without changing the file.")).toBeUndefined();
  });

  it("reads a source file as text context", async () => {
    const attachment = await readAiAttachment(
      new File(["export const value = 42;"], "value.ts", { type: "text/plain" }),
    );

    expect(attachment).toMatchObject({
      fileName: "value.ts",
      mediaType: "text/plain",
      content: "export const value = 42;",
      isImage: false,
    });
  });

  it("formats compact attachment sizes", () => {
    expect(formatAttachmentSize(512)).toBe("512 B");
    expect(formatAttachmentSize(2048)).toBe("2 KB");
  });
});
