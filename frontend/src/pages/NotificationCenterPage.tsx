import { useInfiniteQuery } from "@tanstack/react-query";
import { ErrorState } from "../components/AsyncState";
import { Icon } from "../components/Icon";
import { notificationApi } from "../features/notifications/api";
import { useNotificationStore } from "../features/notifications/notificationStore";
import { usePageTranslation } from "../hooks/usePageTranslation";

export function NotificationCenterPage() {
  const store = useNotificationStore();
  const { pt, locale } = usePageTranslation();
  const query = useInfiniteQuery({
    queryKey: ["notifications"],
    queryFn: ({ pageParam }) => notificationApi.list(pageParam),
    initialPageParam: undefined as string | undefined,
    getNextPageParam: (page) => page.nextCursor,
  });
  const items = query.data?.pages.flatMap((page) => page.items) ?? store.items;
  const unreadCount = items.filter((item) => !item.isRead).length;

  const markAllRead = async () => {
    await notificationApi.readAll();
    store.read();
    await query.refetch();
  };

  return (
    <main className="notification-center">
      <header>
        <div>
          <span>{pt("inbox")}</span>
          <h1>{pt("notifications")}</h1>
          <p>{pt("notificationCenterCopy")}</p>
        </div>
        <button disabled={unreadCount === 0} onClick={() => void markAllRead()}>
          <Icon name="check" />
          {pt("markAllRead")}
        </button>
      </header>

      {query.isError ? (
        <ErrorState message={query.error.message} retry={() => query.refetch()} />
      ) : (
        <section aria-label={pt("notifications")}>
          {query.isLoading && !items.length && (
            <div className="notification-center-loading" aria-label={pt("loadingNotifications")}>
              <i /><i /><i />
            </div>
          )}

          {items.map((item) => (
            <article key={item.id} className={item.isRead ? "" : "unread"}>
              <i aria-hidden="true">{item.type.slice(0, 1)}</i>
              <div>
                <header>
                  <strong>{item.title}</strong>
                  {!item.isRead && <span />}
                </header>
                <p>{item.message}</p>
                <small>{new Date(item.createdAt).toLocaleString(locale)}</small>
              </div>
              {!item.isRead && (
                <button onClick={async () => {
                  await notificationApi.read(item.id);
                  store.read(item.id);
                }}>
                  {pt("markRead")}
                </button>
              )}
            </article>
          ))}

          {!items.length && !query.isLoading && (
            <div className="notification-center-empty">
              <span aria-hidden="true"><Icon name="bell" /></span>
              <h2>{pt("noNotifications")}</h2>
              <p>{pt("notificationEmptyCopy")}</p>
            </div>
          )}

          {query.hasNextPage && (
            <div className="notification-center-more">
              <button disabled={query.isFetchingNextPage} onClick={() => query.fetchNextPage()}>
                {query.isFetchingNextPage ? pt("loadingNotifications") : pt("loadOlder")}
              </button>
            </div>
          )}
        </section>
      )}
    </main>
  );
}
