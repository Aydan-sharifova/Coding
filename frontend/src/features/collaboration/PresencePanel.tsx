import { useMemo } from "react";
import { useCollaborationStore } from "./collaborationStore";

const palette = ["#7c6df2", "#1db7a6", "#e17055", "#0984e3", "#d63075", "#6c5ce7"];
export function userColor(userId: string) {
  let hash = 0; for (const character of userId) hash = ((hash << 5) - hash + character.charCodeAt(0)) | 0;
  return palette[Math.abs(hash) % palette.length];
}
export function PresencePanel() {
  const usersById = useCollaborationStore((state) => state.users);
  const users = useMemo(() => Object.values(usersById), [usersById]);
  const typing = useCollaborationStore((state) => state.typingUserIds);
  return <section className="presence-panel"><header><strong>ONLINE</strong><span>{users.length}</span></header><div className="presence-avatars">
    {users.map((user) => <div className="presence-user" key={user.userId} title={user.displayName}>
      {user.avatarUrl ? <img src={user.avatarUrl} alt="" /> : <span style={{ background: userColor(user.userId) }}>{user.displayName.slice(0, 1).toUpperCase()}</span>}
      <div><b>{user.displayName}</b><small>{typing.includes(user.userId) ? "typing…" : `${user.connectionCount} connection${user.connectionCount === 1 ? "" : "s"}`}</small></div>
    </div>)}{!users.length && <p>No collaborators online.</p>}
  </div></section>;
}
