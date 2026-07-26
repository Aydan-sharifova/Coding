import { useState } from "react";
import { EmptyState, ErrorState, LoadingState } from "../components/AsyncState";
import { Icon } from "../components/Icon";
import { useToast } from "../contexts/ToastContext";
import { ProjectFeatureCard } from "../features/projects/ProjectFeatureCard";
import { ProjectFormDialog } from "../features/projects/ProjectFormDialog";
import { useCreateProject, useProjects } from "../features/projects/hooks";
import type { ProjectInput } from "../features/projects/types";
import { usePageTranslation } from "../hooks/usePageTranslation";

export function ProjectsPage() {
  const [createOpen, setCreateOpen] = useState(false);
  const { pt } = usePageTranslation();
  const projects = useProjects(); const createProject = useCreateProject(); const { show } = useToast();
  const create = async (input: ProjectInput) => { try { await createProject.mutateAsync(input); setCreateOpen(false); show("Project created successfully."); } catch (error) { show(error instanceof Error ? error.message : "Project creation failed.", "error"); } };
  return <main className="dashboard-content feature-page"><header className="feature-heading"><div><p className="dashboard-date">{pt("yourWorkspace")}</p><h1>{pt("projects")}</h1><p>{pt("projectsCopy")}</p></div><button className="create-button" onClick={() => setCreateOpen(true)}><Icon name="plus" /> {pt("createProject")}</button></header>{projects.isLoading ? <LoadingState label={pt("loadingProjects")} /> : projects.isError ? <ErrorState message={projects.error.message} retry={() => projects.refetch()} /> : projects.data?.length ? <section className="feature-project-grid">{projects.data.map((project) => <ProjectFeatureCard key={project.id} project={project} />)}</section> : <EmptyState title={pt("noProjects")} description={pt("firstProject")} action={<button className="ui-button primary" onClick={() => setCreateOpen(true)}>{pt("createProject")}</button>} />}<ProjectFormDialog open={createOpen} pending={createProject.isPending} onClose={() => setCreateOpen(false)} onSubmit={create} /></main>;
}
