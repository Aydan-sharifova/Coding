export type SyncStatus = "connecting" | "connected" | "reconnecting" | "offline" | "synchronizing" | "synchronized" | "failed";

export interface DocumentUpdateMessage {
  projectId: string;
  fileId: string;
  clientId: string;
  updateId: string;
  encodedUpdate: string;
  updateType: "document" | "state" | "restore";
  createdAt: string;
  plainContent?: string;
}

export interface AwarenessUpdateMessage extends Omit<DocumentUpdateMessage, "updateType"> {
  updateType: "awareness";
}

export interface CollaborativeState {
  snapshot?: string;
  updates: DocumentUpdateMessage[];
  sequenceNumber: number;
}

export interface CollaborationUserState {
  userId: string;
  displayName: string;
  avatarUrl?: string;
  color: string;
  activeFile: string;
  isTyping: boolean;
}
