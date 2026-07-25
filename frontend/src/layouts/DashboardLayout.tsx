import { useEffect, useState } from "react";
import { NavLink, Outlet, useNavigate } from "react-router-dom";
import { Icon, type IconName } from "../components/Icon";
import { useAuth } from "../hooks/useAuth";
import { useTheme } from "../hooks/useTheme";
import { NotificationBell } from "../features/notifications/NotificationBell";
import { ProjectFormDialog } from "../features/projects/ProjectFormDialog";
import { useCreateProject, useProjects } from "../features/projects/hooks";
import type { ProjectInput } from "../features/projects/types";
import { useToast } from "../contexts/ToastContext";
import { GlobalSearchPalette } from "../features/search/GlobalSearchPalette";

const navItems: Array<{ label: string; path: string; icon: IconName }> = [
  { label: "Overview", path: "/dashboard", icon: "dashboard" },
  { label: "Projects", path: "/projects", icon: "folder" },
  { label: "Chat", path: "/chat", icon: "team" },
  { label: "Team", path: "/team", icon: "team" },
  { label: "Analytics", path: "/analytics", icon: "chart" },
];

export function DashboardLayout() {
  const [sidebarOpen, setSidebarOpen] = useState(false);
  const [searchOpen, setSearchOpen] = useState(false); const [createOpen, setCreateOpen] = useState(false);
  const { session } = useAuth();
  const { theme, toggleTheme } = useTheme();
  const projects = useProjects(); const createProject = useCreateProject(); const navigate = useNavigate(); const { show } = useToast();
  const user = session?.user;
  const initials = user ? `${user.firstName[0]}${user.lastName[0]}` : "AD";
  useEffect(() => { const shortcut = (event: KeyboardEvent) => { if ((event.metaKey || event.ctrlKey) && event.key.toLowerCase() === "k") { event.preventDefault(); setSearchOpen(true); window.setTimeout(() => document.getElementById("global-search")?.focus(), 0); } }; window.addEventListener("keydown", shortcut); return () => window.removeEventListener("keydown", shortcut); }, []);
  const create = async (input: ProjectInput) => { try { const project = await createProject.mutateAsync(input); setCreateOpen(false); show("Project created successfully."); navigate(`/projects/${project.id}/workspace`); } catch (error) { show(error instanceof Error ? error.message : "Project creation failed.", "error"); } };

  return (
    <div className="dashboard-shell">
      <aside className={`sidebar ${sidebarOpen ? "is-open" : ""}`}>
        <div className="sidebar-brand"><span className="brand-mark">C</span><span>Coding</span></div>
        <nav className="sidebar-nav" aria-label="Main navigation">
          <p>Workspace</p>
          {navItems.map((item) => <NavLink key={item.label} to={item.path} end={item.path === "/dashboard"} onClick={() => setSidebarOpen(false)}><Icon name={item.icon} />{item.label}</NavLink>)}
          {user?.roles.includes("Admin") && <NavLink to="/admin/activity" onClick={() => setSidebarOpen(false)}><Icon name="activity" />Activity</NavLink>}
          <p>Manage</p>
          <NavLink to="/settings"><Icon name="settings" />Settings</NavLink>
          <NavLink to="/help"><Icon name="help" />Help center</NavLink>
        </nav>
        <div className="sidebar-upgrade"><span><Icon name="trend" /></span><strong>Unlock more insights</strong><p>Upgrade your workspace to access advanced analytics.</p><button>View plans</button></div>
        <div className="sidebar-user"><span className="avatar">{initials}</span><div><strong>{user ? `${user.firstName} ${user.lastName}` : "Alex Developer"}</strong><small>{user?.email ?? "alex@coding.dev"}</small></div><Icon name="chevron" /></div>
      </aside>
      {sidebarOpen && <button className="sidebar-backdrop" aria-label="Close navigation" onClick={() => setSidebarOpen(false)} />}
      <div className="dashboard-main">
        <header className="topbar">
          <button className="icon-button mobile-menu" onClick={() => setSidebarOpen(true)} aria-label="Open navigation"><Icon name="menu" /></button>
          <div className="global-search-wrap"><button className="dashboard-search" onClick={() => setSearchOpen(true)} aria-label="Open global search"><Icon name="search" /><span>Search projects, files, users and tasks…</span><kbd>⌘ K</kbd></button></div>
          <div className="topbar-actions">
            <button className="icon-button" onClick={toggleTheme} aria-label={`Switch to ${theme === "dark" ? "light" : "dark"} mode`}><Icon name={theme === "dark" ? "sun" : "moon"} /></button>
            <NotificationBell />
            <button className="create-button" onClick={() => setCreateOpen(true)}><Icon name="plus" /> New project</button>
          </div>
        </header>
        <Outlet />
        <ProjectFormDialog open={createOpen} pending={createProject.isPending} onClose={() => setCreateOpen(false)} onSubmit={create} />
        <GlobalSearchPalette open={searchOpen} onOpenChange={setSearchOpen} />
      </div>
    </div>
  );
}
