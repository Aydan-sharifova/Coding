import type { ProjectSummary } from "../types/dashboard";
import { Icon } from "./Icon";

export function ProjectCard({ project }: { project: ProjectSummary }) {
  return (
    <article className="project-card">
      <div className="project-topline">
        <span className="project-symbol" style={{ background: project.color }}><Icon name="code" /></span>
        <button className="more-button" aria-label={`More options for ${project.name}`}>•••</button>
      </div>
      <h3>{project.name}</h3>
      <p>{project.description}</p>
      <div className="project-meta"><span className="language-dot" style={{ background: project.color }} />{project.language}<span>Updated {project.updatedAt}</span></div>
      <div className="progress-label"><span>Progress</span><strong>{project.progress}%</strong></div>
      <div className="progress-track"><span style={{ width: `${project.progress}%`, background: project.color }} /></div>
    </article>
  );
}
