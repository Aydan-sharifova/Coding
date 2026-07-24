import { useMemo, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";

const articles = [
  { category: "Getting started", title: "Create and configure a project", body: "Open Projects, choose New project, then set its name, language, visibility, and description. Project owners can manage access from Project settings." },
  { category: "Workspace", title: "Work with files and versions", body: "Open a project workspace to create nested files and folders. Changes autosave after a short delay, while version history lets authorized members compare and restore revisions." },
  { category: "Collaboration", title: "Live collaboration and chat", body: "Project members can see presence, remote cursors, typing indicators, workspace channels, and direct conversations. Reconnection restores joined project and file groups." },
  { category: "Planning", title: "Use the Kanban board", body: "Create tasks with priorities, due dates, assignees, and comments. Drag tasks between Todo, Doing, and Done; failed moves automatically roll back." },
  { category: "AI", title: "Use the AI assistant safely", body: "Select editor code and open the AI panel to explain, fix, optimize, or generate tests. AI output never overwrites files automatically and requires confirmation before changing the editor buffer." },
  { category: "Security", title: "Account and access security", body: "Access is protected by JWT authentication and backend authorization. Project resources are available only to active members, and permissions are verified on every protected request." },
  { category: "Troubleshooting", title: "The API is unavailable", body: "Confirm PostgreSQL is running, then start the API on port 5192. The frontend development server normally runs on port 5173 and proxies API and SignalR requests." },
];

export function HelpCenterPage() {
  const [params] = useSearchParams(); const [search, setSearch] = useState(params.get("topic") ?? ""); const [open, setOpen] = useState<string>();
  const results = useMemo(() => articles.filter((item) => `${item.category} ${item.title} ${item.body}`.toLowerCase().includes(search.toLowerCase())), [search]);
  return <main className="dashboard-content help-page"><header className="help-hero"><p className="dashboard-date">HELP CENTER</p><h1>What can we help you build?</h1><p>Search product guidance, collaboration workflows, and troubleshooting steps.</p><label><span>⌕</span><input autoFocus type="search" value={search} onChange={(e) => setSearch(e.target.value)} placeholder="Search help articles…" /></label></header><section className="help-grid">{results.map((article) => <article key={article.title} className={open === article.title ? "open" : ""}><button onClick={() => setOpen(open === article.title ? undefined : article.title)}><span><small>{article.category}</small><b>{article.title}</b></span><i>{open === article.title ? "−" : "+"}</i></button>{open === article.title && <p>{article.body}</p>}</article>)}</section>{!results.length && <div className="feature-state"><strong>No matching articles</strong><p>Try broader words such as project, files, collaboration, or security.</p></div>}<section className="help-contact"><div><h2>Still need help?</h2><p>Review your project settings or return to the dashboard to check service status.</p></div><Link className="ui-button ghost" to="/settings">Account settings</Link><Link className="ui-button primary" to="/dashboard">Dashboard</Link></section></main>;
}
