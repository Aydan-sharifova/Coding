export interface AnalyticsSummary { activeUsers: number; projectsCreated: number; taskCompletionRate: number; fileChanges: number; estimatedCodingHours: number; }
export interface ActiveUser { userId: string; displayName: string; userName: string; avatarUrl?: string; activityCount: number; }
export interface TimeSeriesPoint { period: string; value: number; }
export interface LanguageUsage { language: string; projectCount: number; }
export interface AnalyticsDashboard {
  from: string; to: string; summary: AnalyticsSummary; activeUsers: ActiveUser[];
  projectsOverTime: TimeSeriesPoint[]; languages: LanguageUsage[];
  weeklyActivity: TimeSeriesPoint[]; monthlyActivity: TimeSeriesPoint[];
}
