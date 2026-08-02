import { useEffect, useState } from "react";
import type { Awareness } from "y-protocols/awareness";

export function useCollaborationAwareness(awareness?: Awareness) {
  const [users, setUsers] = useState<Record<string, unknown>[]>([]);
  useEffect(() => {
    if (!awareness) return;
    const update = () => setUsers([...awareness.getStates().values()].map((state) => state.user as Record<string, unknown>).filter(Boolean));
    awareness.on("change", update); update(); return () => awareness.off("change", update);
  }, [awareness]);
  return users;
}
