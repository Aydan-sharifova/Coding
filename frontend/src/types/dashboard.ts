export interface DashboardMetric { key: string; label: string; value: number; displayValue: string; changePercent: number; changeLabel: string; }
export interface DashboardPoint { date: string; contributions: number; }
export interface DashboardActivity { id: string; actionType: string; description: string; entityType: string; entityId?: string; projectName?: string; userName?: string; createdAt: string; }
export interface DashboardProject { id: string; name: string; description?: string; language: string; progress: number; memberCount: number; openTaskCount: number; updatedAt: string; }
export interface DashboardResponse { metrics: DashboardMetric[]; weeklyProgress: DashboardPoint[]; recentActivity: DashboardActivity[]; projects: DashboardProject[]; }
export interface ProjectSummary { id: string; name: string; description: string; language: string; progress: number; updatedAt: string; color: string; }
