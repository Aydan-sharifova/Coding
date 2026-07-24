import { apiClient } from "./apiClient";
import type { DashboardResponse } from "../types/dashboard";
export const dashboardApi = { get: () => apiClient.get<DashboardResponse>("/dashboard") };
