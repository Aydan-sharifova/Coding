import { zodResolver } from "@hookform/resolvers/zod";
import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { Dialog } from "../../components/ui/Dialog";
import type { ProjectInput } from "./types";

const schema = z.object({ name: z.string().trim().min(2, "Use at least 2 characters.").max(120), description: z.string().trim().max(1000).optional(), defaultLanguage: z.string().trim().min(1, "Choose a language.").max(50), isPublic: z.boolean() });

export function ProjectFormDialog({ open, initial, pending, onClose, onSubmit }: { open: boolean; initial?: ProjectInput; pending: boolean; onClose: () => void; onSubmit: (value: ProjectInput) => Promise<void> }) {
  const { register, handleSubmit, reset, formState: { errors } } = useForm<ProjectInput>({ resolver: zodResolver(schema), defaultValues: initial ?? { name: "", description: "", defaultLanguage: "TypeScript", isPublic: false } });
  useEffect(() => reset(initial ?? { name: "", description: "", defaultLanguage: "TypeScript", isPublic: false }), [initial, open, reset]);
  return <Dialog open={open} onClose={onClose} title={initial ? "Edit project" : "Create a project"} description="Set the workspace basics. You can change these later." footer={<><button className="ui-button ghost" onClick={onClose}>Cancel</button><button className="ui-button primary" form="project-form" disabled={pending}>{pending ? "Saving…" : initial ? "Save changes" : "Create project"}</button></>}><form id="project-form" className="feature-form" onSubmit={handleSubmit(onSubmit)}><label>Project name<input {...register("name")} autoFocus />{errors.name && <span>{errors.name.message}</span>}</label><label>Description<textarea {...register("description")} rows={3} />{errors.description && <span>{errors.description.message}</span>}</label><label>Primary language<select {...register("defaultLanguage")}><option>TypeScript</option><option>C#</option><option>Python</option><option>Java</option><option>Go</option><option>Other</option></select></label><label className="check-row"><input type="checkbox" {...register("isPublic")} /><span><strong>Public project</strong><small>Visible to everyone, editable only by members.</small></span></label></form></Dialog>;
}
