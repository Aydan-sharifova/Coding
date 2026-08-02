import type { Awareness } from "y-protocols/awareness";
import { useCollaborationAwareness } from "../hooks/useCollaborationAwareness";

export function RemoteUserList({ awareness }: { awareness?: Awareness }) {
  const users = useCollaborationAwareness(awareness);
  return <div className="remote-user-list">{users.map((user, index) => <span key={String(user.userId ?? index)} style={{ borderColor: String(user.color ?? "currentColor") }} title={String(user.displayName ?? "Collaborator")}>{String(user.displayName ?? "?").slice(0, 1).toUpperCase()}</span>)}</div>;
}
