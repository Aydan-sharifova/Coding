import { apiClient } from "../../services/apiClient";
import type { NotificationPage } from "./types";
export const notificationApi = {
  list: (cursor?: string) => apiClient.get<NotificationPage>(`/notifications?limit=30${cursor ? `&cursor=${encodeURIComponent(cursor)}` : ""}`),
  read: (id: string) => apiClient.put<void>(`/notifications/${id}/read`, {}),
  readAll: () => apiClient.put<void>("/notifications/read-all", {}),
};
