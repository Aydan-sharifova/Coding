import { beforeAll, beforeEach, describe, expect, it } from "vitest";

let useEditorStore: typeof import("./editorStore").useEditorStore;

beforeAll(async () => {
  const values = new Map<string, string>();
  Object.defineProperty(globalThis, "localStorage", {
    configurable: true,
    value: {
      clear: () => values.clear(),
      getItem: (key: string) => values.get(key) ?? null,
      key: (index: number) => [...values.keys()][index] ?? null,
      removeItem: (key: string) => values.delete(key),
      setItem: (key: string, value: string) => values.set(key, value),
      get length() { return values.size; },
    },
  });
  ({ useEditorStore } = await import("./editorStore"));
});

const tab = (id: string, content = "const value = 1;") => ({
  id,
  name: `${id}.ts`,
  path: `/${id}.ts`,
  language: "typescript" as const,
  content,
  savedContent: content,
  concurrencyToken: "0".repeat(32),
});

describe("editorStore", () => {
  beforeEach(() => {
    useEditorStore.setState({
      tabs: {},
      openTabIds: [],
      activeTabId: undefined,
      closedTabHistory: [],
    });
  });

  it("focuses an existing tab instead of duplicating it", () => {
    useEditorStore.getState().openTab(tab("one"));
    useEditorStore.getState().openTab(tab("two"));
    useEditorStore.getState().openTab(tab("one"));

    expect(useEditorStore.getState().openTabIds).toEqual(["one", "two"]);
    expect(useEditorStore.getState().activeTabId).toBe("one");
  });

  it("marks edited content dirty and ignores stale save acknowledgements", () => {
    useEditorStore.getState().openTab(tab("one"));
    useEditorStore.getState().updateContent("one", "newer content");
    const version = useEditorStore.getState().tabs.one.requestVersion;
    useEditorStore.getState().acknowledgeSave("one", version - 1, "old content", "1".repeat(32));

    expect(useEditorStore.getState().tabs.one.content).toBe("newer content");
    expect(useEditorStore.getState().tabs.one.status).toBe("Unsaved");
  });

  it("releases closed file content from memory", () => {
    useEditorStore.getState().openTab(tab("large", "x".repeat(100_000)));
    useEditorStore.getState().closeTab("large");

    expect(useEditorStore.getState().tabs.large).toBeUndefined();
    expect(useEditorStore.getState().closedTabHistory).toEqual(["large"]);
  });
});
