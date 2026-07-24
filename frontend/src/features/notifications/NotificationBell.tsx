import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { Icon } from "../../components/Icon";
import { notificationApi } from "./api";
import { useNotificationStore } from "./notificationStore";
import { signalRService } from "../collaboration/signalRService";

export function NotificationBell() {
  const navigate = useNavigate(); const [open, setOpen] = useState(false);
  const { items, unreadCount, setPage, read } = useNotificationStore();
  useEffect(() => { void signalRService.connect().catch(() => undefined); void notificationApi.list().then((page) => setPage(page.items, page.unreadCount)); }, [setPage]);
  const markRead = async (id: string) => { await notificationApi.read(id); read(id); };
  return <div className="notification-control"><button className="icon-button notification-button" aria-label="Notifications" onClick={() => setOpen((value) => !value)}><Icon name="bell" />{unreadCount > 0 && <span>{unreadCount > 99 ? "99+" : unreadCount}</span>}</button>{open && <div className="notification-dropdown">
    <header><strong>Notifications</strong><button onClick={async () => { await notificationApi.readAll(); read(); }}>Mark all read</button></header>
    <div>{items.slice(0, 6).map((item) => <button key={item.id} className={item.isRead ? "" : "unread"} onClick={() => void markRead(item.id)}><b>{item.title}</b><span>{item.message}</span><small>{new Date(item.createdAt).toLocaleString()}</small></button>)}{!items.length && <p>You're all caught up.</p>}</div>
    <footer><button onClick={() => { setOpen(false); navigate("/notifications"); }}>View notification center</button></footer>
  </div>}</div>;
}
