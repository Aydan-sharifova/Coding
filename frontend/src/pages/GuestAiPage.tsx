import { useEffect, useMemo, useRef, useState } from "react";
import { Link } from "react-router-dom";
import { aiApi } from "../features/ai/api";
import type { GuestAiHistoryMessage } from "../features/ai/types";
import { useAuth } from "../hooks/useAuth";

interface GuestMessage {
  id: string;
  role: "user" | "assistant";
  content: string;
}

const welcomeMessage: GuestMessage = {
  id: "welcome",
  role: "assistant",
  content:
    "Hi, I’m Aydan AI. Ask me a programming question, explore an architecture idea, or bring me an error message.",
};

const suggestedPrompts = [
  "Explain CQRS with a small C# example.",
  "How should I structure a React feature folder?",
  "Help me debug a TypeScript null error.",
];

function MessageContent({ content }: { content: string }) {
  const parts = useMemo(() => {
    const matches = [...content.matchAll(/```([\w+-]*)\n?([\s\S]*?)```/g)];
    if (matches.length === 0) return [{ type: "text", value: content, key: "text" }];

    const result: Array<{ type: "text" | "code"; value: string; key: string }> = [];
    let cursor = 0;
    matches.forEach((match, index) => {
      const start = match.index ?? 0;
      if (start > cursor) {
        result.push({
          type: "text",
          value: content.slice(cursor, start),
          key: `text-${index}`,
        });
      }
      result.push({
        type: "code",
        value: match[2].trim(),
        key: `code-${index}`,
      });
      cursor = start + match[0].length;
    });
    if (cursor < content.length) {
      result.push({
        type: "text",
        value: content.slice(cursor),
        key: "text-last",
      });
    }
    return result;
  }, [content]);

  return (
    <>
      {parts.map((part) =>
        part.type === "code" ? (
          <pre key={part.key}>
            <code>{part.value}</code>
          </pre>
        ) : (
          <p key={part.key}>{part.value}</p>
        ),
      )}
    </>
  );
}

export function GuestAiPage() {
  const { session } = useAuth();
  const [messages, setMessages] = useState<GuestMessage[]>([welcomeMessage]);
  const [draft, setDraft] = useState("");
  const [isStreaming, setIsStreaming] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const abortRef = useRef<AbortController | null>(null);
  const transcriptRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    transcriptRef.current?.scrollTo({
      top: transcriptRef.current.scrollHeight,
      behavior: "smooth",
    });
  }, [messages]);

  useEffect(() => () => abortRef.current?.abort(), []);

  const submit = async (prompt?: string) => {
    const userMessage = (prompt ?? draft).trim();
    if (!userMessage || isStreaming) return;

    const history: GuestAiHistoryMessage[] = messages
      .filter((message) => message.id !== "welcome" && message.content.trim())
      .slice(-8)
      .map((message) => ({
        role: message.role === "user" ? "User" : "Assistant",
        content: message.content,
      }));
    const userId = crypto.randomUUID();
    const assistantId = crypto.randomUUID();
    setMessages((current) => [
      ...current,
      { id: userId, role: "user", content: userMessage },
      { id: assistantId, role: "assistant", content: "" },
    ]);
    setDraft("");
    setError(null);
    setIsStreaming(true);

    const controller = new AbortController();
    abortRef.current = controller;
    try {
      await aiApi.guestStream(
        { userMessage, history },
        controller.signal,
        (chunk) => {
          if (!chunk.content) return;
          setMessages((current) =>
            current.map((message) =>
              message.id === assistantId
                ? { ...message, content: message.content + chunk.content }
                : message,
            ),
          );
        },
      );
    } catch (caught) {
      if (controller.signal.aborted) return;
      const message =
        caught instanceof Error
          ? caught.message
          : "The guest AI preview could not answer right now.";
      setError(message);
      setMessages((current) =>
        current.map((item) =>
          item.id === assistantId && !item.content
            ? {
                ...item,
                content:
                  "I couldn’t complete that response. Please wait a moment and try again.",
              }
            : item,
        ),
      );
    } finally {
      if (abortRef.current === controller) abortRef.current = null;
      setIsStreaming(false);
    }
  };

  const reset = () => {
    abortRef.current?.abort();
    setMessages([welcomeMessage]);
    setDraft("");
    setError(null);
    setIsStreaming(false);
  };

  return (
    <main className="guest-ai-page">
      <header className="guest-ai-nav">
        <Link className="guest-ai-brand" to="/ai">
          <span className="brand-mark">C</span>
          <span>Coding</span>
        </Link>
        <nav aria-label="Guest account options">
          <span className="guest-preview-badge">
            <i aria-hidden="true" />
            Guest preview
          </span>
          {session ? (
            <Link className="guest-nav-primary" to="/dashboard">
              Open workspace
            </Link>
          ) : (
            <>
              <Link className="guest-nav-link" to="/login">
                Sign in
              </Link>
              <Link className="guest-nav-primary" to="/register">
                Create account
              </Link>
            </>
          )}
        </nav>
      </header>

      <section className="guest-ai-layout">
        <aside className="guest-ai-intro">
          <div>
            <p className="eyebrow">No account needed</p>
            <h1>Think through code with an AI pair.</h1>
            <p>
              Start a private, temporary conversation. Register when you want
              project context, file analysis, saved history, and team tools.
            </p>
          </div>
          <ul className="guest-feature-list">
            <li>
              <span>01</span>
              <div>
                <strong>Instant coding help</strong>
                <small>Questions, debugging, and architecture guidance.</small>
              </div>
            </li>
            <li className="locked">
              <span>02</span>
              <div>
                <strong>Project-aware AI</strong>
                <small>Unlock files, images, and repository context.</small>
              </div>
              <b>Account</b>
            </li>
            <li className="locked">
              <span>03</span>
              <div>
                <strong>Collaboration workspace</strong>
                <small>Unlock projects, teams, analytics, and saved work.</small>
              </div>
              <b>Account</b>
            </li>
          </ul>
          {!session && (
            <Link className="guest-intro-cta" to="/register">
              Unlock the full workspace <span>→</span>
            </Link>
          )}
        </aside>

        <section className="guest-chat-shell" aria-label="Guest AI chat">
          <header className="guest-chat-header">
            <div className="guest-ai-avatar" aria-hidden="true">
              AI
            </div>
            <div>
              <strong>Aydan AI</strong>
              <span>
                <i aria-hidden="true" /> Ready to help
              </span>
            </div>
            <button type="button" onClick={reset}>
              New chat
            </button>
          </header>

          <div
            className="guest-chat-transcript"
            ref={transcriptRef}
            aria-live="polite"
          >
            {messages.map((message) => (
              <article className={message.role} key={message.id}>
                <span>{message.role === "assistant" ? "AI" : "You"}</span>
                <div>
                  {message.content ? (
                    <MessageContent content={message.content} />
                  ) : (
                    <div className="guest-typing" aria-label="AI is thinking">
                      <i />
                      <i />
                      <i />
                    </div>
                  )}
                </div>
              </article>
            ))}
            {messages.length === 1 && (
              <div className="guest-prompt-grid">
                {suggestedPrompts.map((prompt) => (
                  <button
                    key={prompt}
                    type="button"
                    onClick={() => void submit(prompt)}
                  >
                    {prompt}
                    <span>↗</span>
                  </button>
                ))}
              </div>
            )}
          </div>

          <footer className="guest-chat-composer">
            {error && <p role="alert">{error}</p>}
            <div>
              <textarea
                aria-label="Message Aydan AI"
                placeholder="Ask about code, bugs, architecture…"
                maxLength={4000}
                rows={1}
                value={draft}
                disabled={isStreaming}
                onChange={(event) => setDraft(event.target.value)}
                onKeyDown={(event) => {
                  if (event.key === "Enter" && !event.shiftKey) {
                    event.preventDefault();
                    void submit();
                  }
                }}
              />
              {isStreaming ? (
                <button
                  className="guest-send-button stop"
                  type="button"
                  aria-label="Stop response"
                  onClick={() => abortRef.current?.abort()}
                >
                  ■
                </button>
              ) : (
                <button
                  className="guest-send-button"
                  type="button"
                  aria-label="Send message"
                  disabled={!draft.trim()}
                  onClick={() => void submit()}
                >
                  ↑
                </button>
              )}
            </div>
            <span>
              Temporary preview · Conversations are not saved · 4,000 character
              limit
            </span>
          </footer>
        </section>
      </section>
    </main>
  );
}
