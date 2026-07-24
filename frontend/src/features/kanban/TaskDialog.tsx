import { useEffect, useState } from "react";
import { Dialog } from "../../components/ui/Dialog";
import type { ProjectTask, TaskInput, TaskPriority } from "./types";

export function TaskDialog({ open, task, pending, onClose, onSubmit }: { open: boolean; task?: ProjectTask; pending: boolean; onClose: () => void; onSubmit: (input: TaskInput) => void }) {
  const [title, setTitle] = useState(""); const [description, setDescription] = useState(""); const [priority, setPriority] = useState<TaskPriority>("Medium"); const [dueDate, setDueDate] = useState("");
  useEffect(() => { if (open) { setTitle(task?.title ?? ""); setDescription(task?.description ?? ""); setPriority(task?.priority ?? "Medium"); setDueDate(task?.dueDate?.slice(0, 10) ?? ""); } }, [open, task]);
  return <Dialog open={open} title={task ? "Edit task" : "Create a task"} description="Add clear, actionable work to the board." onClose={onClose} footer={<><button className="ui-button ghost" onClick={onClose}>Cancel</button><button className="ui-button primary" disabled={pending || !title.trim()} onClick={() => onSubmit({ title: title.trim(), description: description.trim() || undefined, priority, dueDate: dueDate ? new Date(`${dueDate}T12:00:00Z`).toISOString() : null })}>{pending ? "Saving…" : "Save task"}</button></>}>
    <div className="task-form"><label>Title<input value={title} maxLength={200} onChange={(e) => setTitle(e.target.value)} autoFocus /></label><label>Description<textarea value={description} maxLength={4000} onChange={(e) => setDescription(e.target.value)} /></label><div className="task-form-row"><label>Priority<select value={priority} onChange={(e) => setPriority(e.target.value as TaskPriority)}>{["Low", "Medium", "High", "Critical"].map((value) => <option key={value}>{value}</option>)}</select></label><label>Due date<input type="date" value={dueDate} onChange={(e) => setDueDate(e.target.value)} /></label></div></div>
  </Dialog>;
}
