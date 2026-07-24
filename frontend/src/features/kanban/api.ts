import { apiClient } from "../../services/apiClient";
import type { MoveTaskInput, ProjectTask, TaskComment, TaskInput } from "./types";

export const kanbanApi = {
  board: (projectId: string) => apiClient.get<ProjectTask[]>(`/projects/${projectId}/tasks`),
  create: (projectId: string, input: TaskInput) => apiClient.post<ProjectTask>(`/projects/${projectId}/tasks`, input),
  update: (taskId: string, input: TaskInput) => apiClient.put<ProjectTask>(`/tasks/${taskId}`, input),
  remove: (taskId: string) => apiClient.delete<void>(`/tasks/${taskId}`),
  move: (taskId: string, input: MoveTaskInput) => apiClient.put<ProjectTask>(`/tasks/${taskId}/position`, input),
  assign: (taskId: string, userId: string) => apiClient.post<ProjectTask>(`/tasks/${taskId}/assignees/${userId}`),
  unassign: (taskId: string, userId: string) => apiClient.delete<ProjectTask>(`/tasks/${taskId}/assignees/${userId}`),
  comment: (taskId: string, content: string) => apiClient.post<TaskComment>(`/tasks/${taskId}/comments`, { content }),
  deleteComment: (commentId: string) => apiClient.delete<void>(`/task-comments/${commentId}`),
};
