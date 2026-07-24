import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { kanbanApi } from "./api";
import type { MoveTaskInput, TaskInput } from "./types";

export const boardKey = (projectId: string) => ["projects", projectId, "tasks"] as const;
export function useBoard(projectId: string) { return useQuery({ queryKey: boardKey(projectId), queryFn: () => kanbanApi.board(projectId), enabled: Boolean(projectId) }); }
export function useCreateTask(projectId: string) { const q = useQueryClient(); return useMutation({ mutationFn: (input: TaskInput) => kanbanApi.create(projectId, input), onSuccess: () => q.invalidateQueries({ queryKey: boardKey(projectId) }) }); }
export function useUpdateTask(projectId: string, taskId: string) { const q = useQueryClient(); return useMutation({ mutationFn: (input: TaskInput) => kanbanApi.update(taskId, input), onSuccess: () => q.invalidateQueries({ queryKey: boardKey(projectId) }) }); }
export function useMoveTask(projectId: string) {
  const q = useQueryClient();
  return useMutation({
    mutationFn: ({ taskId, input }: { taskId: string; input: MoveTaskInput }) => kanbanApi.move(taskId, input),
    onSettled: () => q.invalidateQueries({ queryKey: boardKey(projectId) }),
  });
}
