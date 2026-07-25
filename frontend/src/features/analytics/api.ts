import { apiClient } from "../../services/apiClient";
import type { AnalyticsDashboard } from "./types";

export const analyticsApi = {
  dashboard: (from: string, to: string, projectId?: string) => {
    const query = new URLSearchParams({ from, to });
    if (projectId) query.set("projectId", projectId);
    return apiClient.get<AnalyticsDashboard>(`/analytics?${query}`);
  },
  startSession: (projectId: string, fileId: string) => apiClient.post<{ sessionId: string }>("/analytics/coding-sessions", { projectId, fileId }),
  heartbeat: (sessionId: string) => apiClient.post<void>(`/analytics/coding-sessions/${sessionId}/heartbeat`),
  endSession: (sessionId: string) => apiClient.post<void>(`/analytics/coding-sessions/${sessionId}/end`),
};
