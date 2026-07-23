import { create } from "zustand";
import type { CollaborationConnectionState, CollaborationUser, CursorPosition } from "./types";

interface RemoteCursor extends CursorPosition { userId: string; }
interface CollaborationState {
  connectionState: CollaborationConnectionState;
  users: Record<string, CollaborationUser>;
  remoteCursors: Record<string, RemoteCursor>;
  typingUserIds: string[];
  liveVersions: Record<string, number>;
  setConnectionState: (state: CollaborationConnectionState) => void;
  setPresence: (users: CollaborationUser[]) => void;
  setCursor: (userId: string, position: CursorPosition) => void;
  setTyping: (userId: string, typing: boolean) => void;
  setLiveVersion: (fileId: string, version: number) => void;
  clearFileState: () => void;
}

export const useCollaborationStore = create<CollaborationState>((set) => ({
  connectionState: "disconnected", users: {}, remoteCursors: {}, typingUserIds: [], liveVersions: {},
  setConnectionState: (connectionState) => set({ connectionState }),
  setPresence: (users) => set({ users: Object.fromEntries(users.map((user) => [user.userId, user])) }),
  setCursor: (userId, position) => set((state) => ({ remoteCursors: { ...state.remoteCursors, [userId]: { ...position, userId } } })),
  setTyping: (userId, typing) => set((state) => ({ typingUserIds: typing ? [...new Set([...state.typingUserIds, userId])] : state.typingUserIds.filter((id) => id !== userId) })),
  setLiveVersion: (fileId, version) => set((state) => ({ liveVersions: { ...state.liveVersions, [fileId]: version } })),
  clearFileState: () => set({ remoteCursors: {}, typingUserIds: [] }),
}));
