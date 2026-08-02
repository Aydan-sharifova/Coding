import { Awareness } from "y-protocols/awareness";
import type * as Y from "yjs";
import { stableUserColor } from "./updateEncoding";

export function createAwareness(doc: Y.Doc, fileId: string, userId: string, displayName: string, avatarUrl?: string) {
  const awareness = new Awareness(doc);
  awareness.setLocalStateField("user", { userId, displayName, avatarUrl, color: stableUserColor(userId), activeFile: fileId, isTyping: false });
  return awareness;
}
