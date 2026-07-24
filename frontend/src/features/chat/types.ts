export interface ChatUser { id: string; userName: string; displayName: string; avatarUrl?: string; }
export interface ChatMessage { id: string; conversationId: string; sender: ChatUser; content: string; createdAt: string; isDeleted: boolean; readByUserIds: string[]; }
export interface Conversation { id: string; type: "Direct" | "ProjectChannel"; projectId?: string; name: string; participants: ChatUser[]; lastMessage?: ChatMessage; unreadCount: number; updatedAt: string; }
export interface MessagePage { items: ChatMessage[]; nextCursor?: string; }
