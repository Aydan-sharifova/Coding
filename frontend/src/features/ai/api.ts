import { apiClient, ApiError } from "../../services/apiClient";
import { tokenStore } from "../../services/tokenStore";
import type { AiAssistantRequest, AiConversation, AiConversationDetails, AiStreamChunk } from "./types";

const API_URL = import.meta.env.VITE_API_URL ?? "/api";
export const aiApi = {
  conversations: (projectId: string) => apiClient.get<AiConversation[]>(`/ai/projects/${projectId}/conversations`),
  conversation: (id: string) => apiClient.get<AiConversationDetails>(`/ai/conversations/${id}`),
  async stream(request: AiAssistantRequest, signal: AbortSignal, onChunk: (chunk: AiStreamChunk) => void) {
    const send = () => fetch(`${API_URL}/ai/stream`, { method: "POST", credentials: "include", signal, headers: { "Content-Type": "application/json", Authorization: `Bearer ${tokenStore.get() ?? ""}` }, body: JSON.stringify(request) });
    let response = await send();
    if (response.status === 401) { await apiClient.refreshSession(); response = await send(); }
    if (!response.ok || !response.body) throw new ApiError(`AI request failed (${response.status}).`, response.status);
    const reader = response.body.getReader(); const decoder = new TextDecoder(); let buffer = "";
    while (true) {
      const { done, value } = await reader.read(); if (done) break; buffer += decoder.decode(value, { stream: true });
      const events = buffer.split("\n\n"); buffer = events.pop() ?? "";
      for (const event of events) {
        const type = event.split("\n").find((line) => line.startsWith("event:"))?.slice(6).trim();
        const data = event.split("\n").filter((line) => line.startsWith("data:")).map((line) => line.slice(5).trim()).join("\n");
        if (!data) continue; if (type === "error") throw new Error((JSON.parse(data) as { message?: string }).message ?? "AI generation failed.");
        onChunk(JSON.parse(data) as AiStreamChunk);
      }
    }
  },
};
