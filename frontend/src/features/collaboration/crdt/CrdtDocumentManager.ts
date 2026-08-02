import { IndexeddbPersistence } from "y-indexeddb";
import { Awareness } from "y-protocols/awareness";
import * as Y from "yjs";
import { SignalRYjsProvider } from "./SignalRYjsProvider";

export interface ManagedDocument { doc: Y.Doc; text: Y.Text; awareness: Awareness; provider: SignalRYjsProvider; persistence: IndexeddbPersistence; undoManager: Y.UndoManager; localOrigin: object; consumers: number; }

class CrdtDocumentManager {
  private documents = new Map<string, ManagedDocument>();
  acquire(projectId: string, fileId: string, initialContent: string): ManagedDocument {
    const existing = this.documents.get(fileId); if (existing) { existing.consumers += 1; return existing; }
    const doc = new Y.Doc(); const text = doc.getText("monaco"); const localOrigin = {};
    const persistence = new IndexeddbPersistence(`coding:${projectId}:${fileId}`, doc);
    persistence.once("synced", () => { if (text.length === 0 && initialContent) doc.transact(() => text.insert(0, initialContent), localOrigin); });
    const awareness = new Awareness(doc);
    awareness.setLocalStateField("user", { userId: `client-${doc.clientID}`, displayName: "Collaborator", color: `hsl(${doc.clientID % 360} 72% 58%)`, activeFile: fileId, isTyping: false });
    const provider = new SignalRYjsProvider(projectId, fileId, String(doc.clientID), doc, awareness);
    const managed = { doc, text, awareness, provider, persistence, undoManager: new Y.UndoManager(text), localOrigin, consumers: 1 };
    this.documents.set(fileId, managed); void provider.connect(); return managed;
  }
  release(fileId: string) { const managed = this.documents.get(fileId); if (!managed || --managed.consumers > 0) return; this.documents.delete(fileId); void managed.provider.destroy(); managed.persistence.destroy(); managed.undoManager.destroy(); managed.awareness.destroy(); managed.doc.destroy(); }
  get(fileId: string) { return this.documents.get(fileId); }
  reset(fileId: string, content: string) { const managed = this.documents.get(fileId); if (!managed) return; managed.doc.transact(() => { managed.text.delete(0, managed.text.length); managed.text.insert(0, content); }, managed.localOrigin); }
  size() { return this.documents.size; }
}

export const crdtDocumentManager = new CrdtDocumentManager();
export { CrdtDocumentManager };
