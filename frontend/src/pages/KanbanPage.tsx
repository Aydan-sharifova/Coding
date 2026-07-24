import { useState } from "react";
import { useQueryClient } from "@tanstack/react-query";
import { useParams } from "react-router-dom";
import { EmptyState, ErrorState, LoadingState } from "../components/AsyncState";
import { Icon } from "../components/Icon";
import { useToast } from "../contexts/ToastContext";
import { useAuth } from "../hooks/useAuth";
import { KanbanBoard } from "../features/kanban/KanbanBoard";
import { TaskDialog } from "../features/kanban/TaskDialog";
import { TaskDrawer } from "../features/kanban/TaskDrawer";
import { boardKey, useBoard, useCreateTask, useMoveTask, useUpdateTask } from "../features/kanban/hooks";
import type { ProjectTask, TaskInput, TaskStatus } from "../features/kanban/types";
import { useProject, useProjectMembers } from "../features/projects/hooks";

export function KanbanPage() {
  const { projectId = "" } = useParams(); const board = useBoard(projectId); const project = useProject(projectId); const members = useProjectMembers(projectId);
  const { session } = useAuth();
  const create = useCreateTask(projectId); const move = useMoveTask(projectId); const query = useQueryClient(); const { show } = useToast();
  const [dialogOpen, setDialogOpen] = useState(false); const [selectedId, setSelectedId] = useState<string>(); const [editing, setEditing] = useState<ProjectTask>();
  const tasks = board.data ?? []; const selected = tasks.find((x) => x.id === selectedId); const update = useUpdateTask(projectId, editing?.id ?? "");
  const canManage = project.data?.currentUserRole === "Owner" || project.data?.currentUserRole === "Admin" || selected?.createdByUserId === session?.user.id;
  const save = async (input: TaskInput) => { try { if (editing) await update.mutateAsync(input); else await create.mutateAsync(input); setDialogOpen(false); setEditing(undefined); show(editing ? "Task updated." : "Task created."); } catch (e) { show(e instanceof Error ? e.message : "Could not save task.", "error"); } };
  const moveTask = (taskId: string, status: TaskStatus, previousTaskId?: string, nextTaskId?: string) => {
    const snapshot = query.getQueryData<ProjectTask[]>(boardKey(projectId)); if (!snapshot) return;
    const optimistic = snapshot.map((x) => x.id === taskId ? { ...x, status, position: nextTaskId ? (snapshot.find((n) => n.id === nextTaskId)?.position ?? x.position) - 0.5 : (snapshot.filter((n) => n.status === status).at(-1)?.position ?? 0) + 1024 } : x);
    query.setQueryData(boardKey(projectId), optimistic);
    move.mutate({ taskId, input: { status, previousTaskId, nextTaskId } }, { onError: (e) => { query.setQueryData(boardKey(projectId), snapshot); show(e instanceof Error ? e.message : "Move failed.", "error"); } });
  };
  if (board.isLoading || project.isLoading) return <LoadingState label="Loading project board…" />;
  if (board.isError) return <ErrorState message={board.error.message} retry={() => board.refetch()} />;
  return <main className="dashboard-content kanban-page"><header className="feature-heading"><div><p className="dashboard-date">PROJECT BOARD</p><h1>{project.data?.name ?? "Kanban"}</h1><p>Plan, prioritize, and ship work with your team.</p></div><button className="create-button" onClick={() => { setEditing(undefined); setDialogOpen(true); }}><Icon name="plus" /> New task</button></header>{tasks.length ? <KanbanBoard tasks={tasks} onMove={moveTask} onOpen={(task) => setSelectedId(task.id)} /> : <EmptyState title="Your board is ready" description="Create the first task to start planning work." action={<button className="ui-button primary" onClick={() => setDialogOpen(true)}>Create task</button>} />}<TaskDialog open={dialogOpen} task={editing} pending={create.isPending || update.isPending} onClose={() => { setDialogOpen(false); setEditing(undefined); }} onSubmit={(input) => void save(input)} /><TaskDrawer task={selected} members={members.data ?? []} canManage={Boolean(canManage)} onClose={() => setSelectedId(undefined)} onEdit={() => { if (selected) { setEditing(selected); setDialogOpen(true); } }} /></main>;
}
