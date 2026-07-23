export type CollaborationConnectionState = "connected" | "reconnecting" | "disconnected" | "failed";
export interface TextRange { startLineNumber: number; startColumn: number; endLineNumber: number; endColumn: number; }
export interface CodeOperation { operationId: string; fileId: string; userId: string; clientVersion: number; baseVersion: number; range: TextRange; insertedText: string; deletedLength: number; timestamp: string; }
export interface CursorPosition { fileId: string; lineNumber: number; column: number; selection?: TextRange; }
export interface CollaborationUser { userId: string; userName: string; displayName: string; avatarUrl?: string; connectionCount: number; lastSeenAt: string; }
export interface PresenceUpdate { projectId: string; users: CollaborationUser[]; }
export interface FileChangedMessage { fileId: string; changedByUserId: string; versionNumber: number; concurrencyToken: string; }
export interface ResyncRequiredMessage { fileId: string; serverVersion: number; reason: string; }
