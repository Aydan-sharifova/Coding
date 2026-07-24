export type AiAction = "Chat" | "Explain" | "FindBug" | "SuggestFix" | "Optimize" | "GenerateTests" | "Refactor";
export type AiRole = "System" | "User" | "Assistant";
export interface AiAssistantRequest { projectId: string; userMessage: string; action: AiAction; conversationId?: string; currentFileId?: string; selectedCode?: string; neighboringCode?: string; programmingLanguage?: string; referencedFileIds?: string[]; }
export interface AiStreamChunk { content: string; isCompleted: boolean; inputTokens?: number; outputTokens?: number; finishReason?: string; conversationId?: string; }
export interface AiConversation { id: string; projectId: string; title: string; createdAt: string; updatedAt: string; }
export interface AiMessage { id: string; role: AiRole; content: string; action?: AiAction; fileId?: string; createdAt: string; }
export interface AiConversationDetails { conversation: AiConversation; messages: AiMessage[]; }
