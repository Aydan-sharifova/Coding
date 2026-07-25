import { useSyncExternalStore } from "react";
import type { WorkspaceNode } from "./types";

class ExplorerStore {
  entities = new Map<string, WorkspaceNode>(); children = new Map<string, string[]>(); expanded = new Set<string>(); selectedId?: string;
  private listeners = new Set<() => void>(); private current = { entities: this.entities, children: this.children, expanded: this.expanded, selectedId: this.selectedId };
  private loadedTreeSignature = "";
  subscribe = (listener: () => void) => {
    this.listeners.add(listener);
    return () => {
      this.listeners.delete(listener);
    };
  };
  private emit() { this.current = { entities: this.entities, children: this.children, expanded: this.expanded, selectedId: this.selectedId }; this.listeners.forEach((listener) => listener()); }
  load(nodes: WorkspaceNode[]) {
    const signature = nodes
      .map((node) => `${node.id}:${node.parentId ?? ""}:${node.name}:${node.nodeType}:${node.path}`)
      .sort()
      .join("|");

    if (signature === this.loadedTreeSignature) return;

    this.loadedTreeSignature = signature;
    this.entities = new Map(nodes.map((node) => [node.id, node]));
    this.children = new Map();
    for (const node of nodes) {
      const key = node.parentId ?? "root";
      this.children.set(key, [...(this.children.get(key) ?? []), node.id]);
    }
    this.emit();
  }
  toggle(id: string) { const next = new Set(this.expanded); next.has(id) ? next.delete(id) : next.add(id); this.expanded = next; this.emit(); }
  select(id: string) { this.selectedId = id; this.emit(); }
  snapshot = () => this.current;
}
export const explorerStore = new ExplorerStore();
export function useExplorerSnapshot() { return useSyncExternalStore(explorerStore.subscribe, explorerStore.snapshot, explorerStore.snapshot); }
