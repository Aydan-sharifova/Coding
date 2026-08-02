import { useEffect, useMemo } from "react";
import { crdtDocumentManager } from "../crdt/CrdtDocumentManager";

export function useCollaborativeDocument(projectId: string, fileId: string, initialContent: string) {
  const managed = useMemo(() => crdtDocumentManager.acquire(projectId, fileId, initialContent), [projectId, fileId]);
  useEffect(() => () => crdtDocumentManager.release(fileId), [fileId]);
  return managed;
}
