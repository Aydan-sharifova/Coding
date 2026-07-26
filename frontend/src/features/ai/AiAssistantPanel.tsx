import { useEffect, useRef, useState } from "react";
import { aiApi } from "./api";
import type { AiAction, AiMessage } from "./types";

interface AiAssistantPanelProps {
  projectId: string;
  fileId?: string;
  fileName?: string;
  language?: string;
  selectedCode?: string;
  fileContent?: string;
  contextText?: string;
  contextLabel?: string;
  onApplySuggestion: (content: string) => void;
}

const actions: Array<{ action: AiAction; label: string }> = [
  { action: "GenerateCode", label: "Generate" },
  { action: "Explain", label: "Explain" },
  { action: "FindBug", label: "Find bug" },
  { action: "SuggestFix", label: "Fix" },
  { action: "Optimize", label: "Optimize" },
  { action: "GenerateTests", label: "Tests" },
  { action: "Refactor", label: "Refactor" },
];

export function AiAssistantPanel({
  projectId,
  fileId,
  fileName,
  language,
  selectedCode,
  fileContent,
  contextText,
  contextLabel,
  onApplySuggestion,
}: AiAssistantPanelProps) {
  const [messages, setMessages] = useState<AiMessage[]>([]);
  const [message, setMessage] = useState("");
  const [conversationId, setConversationId] = useState<string>();
  const [streaming, setStreaming] = useState(false);
  const [error, setError] = useState<string>();
  const controller = useRef<AbortController | undefined>(undefined);
  const endRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    return () => {
      controller.current?.abort();
    };
  }, []);
  useEffect(() => {
    endRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages]);

  const submit = async (action: AiAction, explicitMessage?: string) => {
    const userMessage = (explicitMessage ?? message).trim();
    if (!userMessage && !selectedCode && action === "Chat") return;
    if (!userMessage && !selectedCode && !fileId && !contextText) return;
    controller.current?.abort();
    controller.current = new AbortController();
    setStreaming(true);
    setError(undefined);
    setMessage("");

    const user: AiMessage = {
      id: crypto.randomUUID(),
      role: "User",
      content: userMessage || `${action} using the ${
        selectedCode ? "selected code" : contextText ? "available project context" : "current file"
      }.`,
      action,
      fileId,
      createdAt: new Date().toISOString(),
    };
    const assistantId = crypto.randomUUID();
    setMessages((current) => [...current, user, { id: assistantId, role: "Assistant", content: "", action, fileId, createdAt: new Date().toISOString() }]);

    try {
      await aiApi.stream({
        projectId,
        userMessage: user.content,
        action,
        conversationId,
        currentFileId: fileId,
        selectedCode,
        neighboringCode: selectedCode
          ? fileContent?.slice(0, 4_000)
          : contextText?.slice(0, 4_000),
        programmingLanguage: language,
      }, controller.current.signal, (chunk) => {
        if (chunk.conversationId) setConversationId(chunk.conversationId);
        if (chunk.content) {
          setMessages((current) => current.map((item) =>
            item.id === assistantId ? { ...item, content: item.content + chunk.content } : item));
        }
      });
    } catch (reason) {
      if (!controller.current.signal.aborted)
        setError(reason instanceof Error ? reason.message : "AI generation failed.");
    } finally {
      setStreaming(false);
    }
  };

  const lastAssistant = [...messages].reverse().find((item) => item.role === "Assistant" && item.content);
  const hasContext = Boolean(fileId || selectedCode || fileContent || contextText);
  return (
    <section className="ai-assistant-panel" aria-label="AI assistant">
      <header>
        <div>
          <span className="ai-mark">AI</span>
          <div><strong>Assistant</strong><small>{fileName ? `Context: ${fileName}` : "Open a file for code context"}</small></div>
        </div>
        {streaming && <button onClick={() => controller.current?.abort()}>Stop</button>}
      </header>

      <div className="ai-actions">
        {actions.map((item) => (
          <button key={item.action} disabled={!hasContext && item.action !== "Chat"} onClick={() => void submit(item.action)}>
            {item.label}
          </button>
        ))}
      </div>

      {(selectedCode || contextText) && (
        <div className="ai-context-chip">
          <span>{selectedCode ? "Selected code" : contextLabel ?? "Project context"}</span>
          <b>{(selectedCode ?? contextText ?? "").length} chars</b>
        </div>
      )}

      <div className="ai-messages" aria-live="polite">
        {!messages.length && (
          <div className="ai-welcome">
            <span>✦</span>
            <strong>Ask about your code</strong>
            <p>Generate, explain, fix, optimize, refactor, test, or ask about project context.</p>
          </div>
        )}
        {messages.map((item) => (
          <article key={item.id} className={item.role.toLowerCase()}>
            <small>{item.role === "Assistant" ? "AI" : "You"}</small>
            <p>{item.content || (streaming ? "Thinking…" : "")}</p>
            {item.role === "Assistant" && item.content && (
              <div>
                <button onClick={() => void navigator.clipboard.writeText(item.content)}>Copy</button>
                {fileId && <button onClick={() => onApplySuggestion(item.content)}>Apply…</button>}
              </div>
            )}
          </article>
        ))}
        {error && <div className="ai-error" role="alert">{error}<button onClick={() => setError(undefined)}>×</button></div>}
        <div ref={endRef} />
      </div>

      <footer>
        <textarea
          value={message}
          onChange={(event) => setMessage(event.target.value)}
          onKeyDown={(event) => {
            if (event.key === "Enter" && !event.shiftKey) {
              event.preventDefault();
              if (!streaming) void submit("Chat");
            }
          }}
          placeholder={fileId ? "Ask about this file…" : "Ask a software question…"}
        />
        <button disabled={streaming || (!message.trim() && !selectedCode)} onClick={() => void submit("Chat")}>Send</button>
      </footer>
      {lastAssistant && <small className="ai-safety-note">AI suggestions are never applied without confirmation.</small>}
    </section>
  );
}
