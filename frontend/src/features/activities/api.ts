import { apiClient } from "../../services/apiClient";
export interface ActivityLog { id: string; userId?: string; userName?: string; projectId?: string; projectName?: string; actionType: string; entityType: string; entityId?: string; description: string; metadata: Record<string, unknown>; ipAddress?: string; userAgent?: string; createdAt: string; }
export interface ActivityPage { items: ActivityLog[]; total: number; page: number; pageSize: number; }
export interface ActivityFilters { userId?: string; projectId?: string; actionType?: string; entityType?: string; from?: string; to?: string; page?: number; }
export const activityApi = { list: (filters: ActivityFilters) => { const query = new URLSearchParams(); Object.entries(filters).forEach(([key, value]) => value && query.set(key, String(value))); return apiClient.get<ActivityPage>(`/admin/activities?${query}`); } };
