export type TaskStatus = "Todo" | "Doing" | "Done";
export type TaskPriority = "Low" | "Medium" | "High" | "Critical";
export interface TaskAssignee { userId: string; displayName: string; avatarUrl?: string; }
export interface TaskComment { id: string; userId: string; displayName: string; avatarUrl?: string; content: string; createdAt: string; }
export interface ProjectTask {
  id: string; projectId: string; title: string; description?: string; status: TaskStatus;
  priority: TaskPriority; position: number; dueDate?: string; createdByUserId: string;
  createdAt: string; updatedAt: string; assignees: TaskAssignee[]; comments: TaskComment[];
}
export interface TaskInput { title: string; description?: string; priority: TaskPriority; dueDate?: string | null; }
export interface MoveTaskInput { status: TaskStatus; previousTaskId?: string | null; nextTaskId?: string | null; }
