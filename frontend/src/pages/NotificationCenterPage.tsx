import { useInfiniteQuery } from "@tanstack/react-query";
import { notificationApi } from "../features/notifications/api";
import { useNotificationStore } from "../features/notifications/notificationStore";

export function NotificationCenterPage() {
  const store = useNotificationStore();
  const query = useInfiniteQuery({ queryKey: ["notifications"], queryFn: ({ pageParam }) => notificationApi.list(pageParam), initialPageParam: undefined as string | undefined, getNextPageParam: (page) => page.nextCursor });
  const items = query.data?.pages.flatMap((page) => page.items) ?? store.items;
  return <main className="notification-center"><header><div><span>INBOX</span><h1>Notifications</h1><p>Project activity, mentions and direct messages.</p></div><button onClick={async () => { await notificationApi.readAll(); store.read(); await query.refetch(); }}>Mark all as read</button></header><section>
    {items.map((item) => <article key={item.id} className={item.isRead ? "" : "unread"}><i>{item.type.slice(0, 1)}</i><div><strong>{item.title}</strong><p>{item.message}</p><small>{new Date(item.createdAt).toLocaleString()}</small></div>{!item.isRead && <button onClick={async () => { await notificationApi.read(item.id); store.read(item.id); }}>Mark read</button>}</article>)}
    {!items.length && !query.isLoading && <div className="empty-state"><h2>No notifications</h2><p>New activity will appear here.</p></div>}
    {query.hasNextPage && <button className="load-older" onClick={() => query.fetchNextPage()}>Load older</button>}
  </section></main>;
}
