import { beforeEach, describe, expect, it, vi } from "vitest";
import * as Y from "yjs";

vi.mock("y-indexeddb", () => ({ IndexeddbPersistence: class { once(_event: string, callback: () => void) { callback(); } destroy() {} } }));
vi.mock("./SignalRYjsProvider", () => ({ SignalRYjsProvider: class { status = "synchronized"; async connect() {} async destroy() {} } }));

describe("CRDT document lifecycle", () => {
  beforeEach(() => vi.resetModules());
  it("reuses a Y.Doc and disposes it after the final consumer", async () => {
    const { CrdtDocumentManager } = await import("./CrdtDocumentManager"); const manager = new CrdtDocumentManager();
    const first = manager.acquire("project", "file", "hello"); const second = manager.acquire("project", "file", "ignored");
    expect(second.doc).toBe(first.doc); expect(manager.size()).toBe(1);
    manager.release("file"); expect(manager.size()).toBe(1); manager.release("file"); expect(manager.size()).toBe(0);
  });

  it("keeps concurrent and offline edits after reconnection", () => {
    const a = new Y.Doc(); const b = new Y.Doc(); const aText = a.getText("monaco"); const bText = b.getText("monaco");
    aText.insert(0, "base"); Y.applyUpdate(b, Y.encodeStateAsUpdate(a));
    aText.insert(4, " from A"); bText.insert(0, "B: ");
    const aOffline = Y.encodeStateAsUpdate(a, Y.encodeStateVector(b)); const bWhileOffline = Y.encodeStateAsUpdate(b, Y.encodeStateVector(a));
    Y.applyUpdate(a, bWhileOffline); Y.applyUpdate(b, aOffline);
    expect(aText.toString()).toBe(bText.toString()); expect(aText.toString()).toContain("from A"); expect(aText.toString()).toContain("B:");
  });

  it("does not put remote updates into the local undo stack", () => {
    const local = new Y.Doc(); const remote = new Y.Doc(); const text = local.getText("monaco"); const undo = new Y.UndoManager(text); const remoteOrigin = {};
    text.insert(0, "local"); Y.applyUpdate(remote, Y.encodeStateAsUpdate(local)); remote.getText("monaco").insert(5, " remote"); Y.applyUpdate(local, Y.encodeStateAsUpdate(remote, Y.encodeStateVector(local)), remoteOrigin);
    undo.undo(); expect(text.toString()).toBe(" remote");
  });
});
