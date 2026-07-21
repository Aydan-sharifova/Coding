import { Icon } from "../components/Icon";
import { ProjectCard } from "../components/ProjectCard";
import { StatCard } from "../components/StatCard";
import { WeeklyProgressChart } from "../components/WeeklyProgressChart";
import type { ActivityItem, ProjectSummary } from "../types/dashboard";

const projects: ProjectSummary[] = [
  { id: "1", name: "API Gateway", description: "Unified entry point for platform services and authentication.", language: "C#", progress: 78, updatedAt: "2h ago", color: "#6c5ce7" },
  { id: "2", name: "Developer Portal", description: "Documentation and tools for an excellent developer experience.", language: "TypeScript", progress: 62, updatedAt: "5h ago", color: "#3182f6" },
  { id: "3", name: "Analytics Engine", description: "Real-time product metrics and performance intelligence.", language: "Python", progress: 91, updatedAt: "Yesterday", color: "#13b981" },
];

const activities: ActivityItem[] = [
  { id: "1", title: "You pushed 4 commits", detail: "to API Gateway · main", time: "12 min ago", tone: "purple" },
  { id: "2", title: "Maya opened a pull request", detail: "Improve dashboard loading states", time: "48 min ago", tone: "blue" },
  { id: "3", title: "Deployment completed", detail: "Developer Portal · Production", time: "2 hours ago", tone: "green" },
  { id: "4", title: "Code review requested", detail: "Analytics Engine · PR #184", time: "4 hours ago", tone: "orange" },
];

export function DashboardPage() {
  return (
    <main className="dashboard-content">
      <header className="dashboard-heading"><div><p className="dashboard-date">MONDAY, JULY 21</p><h1>Good morning, Alex <span>👋</span></h1><p>Here’s what’s happening across your workspace today.</p></div><button className="secondary-button"><Icon name="chart" /> View report</button></header>

      <section className="stats-grid" aria-label="Workspace statistics">
        <StatCard label="Active projects" value="12" change="8% this month" icon="folder" tone="purple" />
        <StatCard label="Commits this week" value="127" change="14% from last week" icon="code" tone="blue" />
        <StatCard label="Coding hours" value="36.5h" change="4.2h this week" icon="activity" tone="green" />
        <StatCard label="Team velocity" value="84%" change="6% improvement" icon="trend" tone="orange" />
      </section>

      <section className="dashboard-grid">
        <article className="panel progress-panel">
          <div className="panel-heading"><div><h2>Weekly progress</h2><p>Your coding activity over the past 7 days</p></div><select aria-label="Select weekly progress period" defaultValue="week"><option value="week">This week</option><option value="month">This month</option></select></div>
          <WeeklyProgressChart />
        </article>
        <article className="panel activity-panel">
          <div className="panel-heading"><div><h2>Recent activity</h2><p>Latest updates from your team</p></div><button>View all</button></div>
          <div className="activity-list">
            {activities.map((activity) => <div className="activity-item" key={activity.id}><span className={`activity-icon ${activity.tone}`}><Icon name={activity.tone === "green" ? "trend" : activity.tone === "blue" ? "folder" : "code"} /></span><div><strong>{activity.title}</strong><p>{activity.detail}</p></div><time>{activity.time}</time></div>)}
          </div>
        </article>
      </section>

      <section className="projects-section">
        <div className="section-heading"><div><h2>Projects</h2><p>Continue where you left off</p></div><button>View all projects <Icon name="chevron" /></button></div>
        <div className="projects-grid">{projects.map((project) => <ProjectCard key={project.id} project={project} />)}</div>
      </section>
    </main>
  );
}
