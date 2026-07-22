import { useQueryClient } from "@tanstack/react-query";
import { fileExplorerApi } from "../../fileExplorer/api";
import type { WorkspaceNode } from "../../fileExplorer/types";
import { detectLanguage } from "../languages";
import { useEditorStore } from "../editorStore";

export function useEditorTabs() {
  const queryClient = useQueryClient(); const store = useEditorStore();
  const openFile = async (node: WorkspaceNode) => {
    if (store.tabs[node.id]) { store.activateTab(node.id); return; }
    const file = await queryClient.fetchQuery({ queryKey: ["file-content", node.id], queryFn: () => fileExplorerApi.content(node.id) });
    store.openTab({ id: node.id, name: node.name, path: file.path, language: detectLanguage(node.name), content: file.content, savedContent: file.content, concurrencyToken: file.concurrencyToken });
  };
  return { ...store, openFile, activeTab: store.activeTabId ? store.tabs[store.activeTabId] : undefined };
}
