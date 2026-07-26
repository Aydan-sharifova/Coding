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
import { useLanguage } from "../hooks/useLanguage";
import type { TranslationKey } from "../contexts/LanguageContext";
import { usePageTranslation } from "../hooks/usePageTranslation";

const navItems: Array<{ label: TranslationKey; path: string; icon: IconName }> = [
  { label: "overview", path: "/dashboard", icon: "dashboard" },
  { label: "projects", path: "/projects", icon: "folder" },
  { label: "chat", path: "/chat", icon: "team" },
  { label: "team", path: "/team", icon: "team" },
  { label: "analytics", path: "/analytics", icon: "chart" },
];

export function DashboardLayout() {
  const [sidebarOpen, setSidebarOpen] = useState(false);
  const [searchOpen, setSearchOpen] = useState(false); const [createOpen, setCreateOpen] = useState(false);
  const { session } = useAuth();
  const { theme, toggleTheme } = useTheme();
  const { t } = useLanguage();
  const { pt } = usePageTranslation();
  const projects = useProjects(); const createProject = useCreateProject(); const navigate = useNavigate(); const { show } = useToast();
  const user = session?.user;
  const initials = user ? `${user.firstName[0]}${user.lastName[0]}` : "AD";
  useEffect(() => { const shortcut = (event: KeyboardEvent) => { if ((event.metaKey || event.ctrlKey) && event.key.toLowerCase() === "k") { event.preventDefault(); setSearchOpen(true); window.setTimeout(() => document.getElementById("global-search")?.focus(), 0); } }; window.addEventListener("keydown", shortcut); return () => window.removeEventListener("keydown", shortcut); }, []);
  const create = async (input: ProjectInput) => { try { const project = await createProject.mutateAsync(input); setCreateOpen(false); show("Project created successfully."); navigate(`/projects/${project.id}/workspace`); } catch (error) { show(error instanceof Error ? error.message : "Project creation failed.", "error"); } };

  return (
    <div className="dashboard-shell">
      <aside className={`sidebar ${sidebarOpen ? "is-open" : ""}`}>
        <div className="sidebar-brand"><span className="brand-mark">C</span><span>Coding</span></div>
        <nav className="sidebar-nav" aria-label={t("openNavigation")}>
          <p>{t("workspace")}</p>
          {navItems.map((item) => <NavLink key={item.label} to={item.path} end={item.path === "/dashboard"} onClick={() => setSidebarOpen(false)}><Icon name={item.icon} />{t(item.label)}</NavLink>)}
          {user?.roles.some((role) => ["SuperAdmin", "Admin"].includes(role)) && <><NavLink to="/admin" onClick={() => setSidebarOpen(false)}><Icon name="settings" />{t("admin")}</NavLink><NavLink to="/admin/activity" onClick={() => setSidebarOpen(false)}><Icon name="activity" />{t("activity")}</NavLink></>}
          <p>{t("manage")}</p>
          <NavLink to="/settings"><Icon name="settings" />{t("settings")}</NavLink>
          <NavLink to="/help"><Icon name="help" />{t("help")}</NavLink>
        </nav>
        <div className="sidebar-upgrade"><span><Icon name="trend" /></span><strong>{pt("unlockInsights")}</strong><p>{pt("upgradeWorkspace")}</p><button>{pt("viewPlans")}</button></div>
        <div className="sidebar-user"><span className="avatar">{initials}</span><div><strong>{user ? `${user.firstName} ${user.lastName}` : "Alex Developer"}</strong><small>{user?.email ?? "alex@coding.dev"}</small></div><Icon name="chevron" /></div>
      </aside>
      {sidebarOpen && <button className="sidebar-backdrop" aria-label={t("closeNavigation")} onClick={() => setSidebarOpen(false)} />}
      <div className="dashboard-main">
        <header className="topbar">
          <button className="icon-button mobile-menu" onClick={() => setSidebarOpen(true)} aria-label={t("openNavigation")}><Icon name="menu" /></button>
          <div className="global-search-wrap"><button className="dashboard-search" onClick={() => setSearchOpen(true)} aria-label={t("search")}><Icon name="search" /><span>{t("search")}</span><kbd>⌘ K</kbd></button></div>
          <div className="topbar-actions">
            <button className="icon-button" onClick={toggleTheme} aria-label={t("theme")}><Icon name={theme === "dark" ? "sun" : "moon"} /></button>
            <NotificationBell />
            <button className="create-button" onClick={() => setCreateOpen(true)}><Icon name="plus" /> {t("newProject")}</button>
          </div>
        </header>
        <Outlet />
        <ProjectFormDialog open={createOpen} pending={createProject.isPending} onClose={() => setCreateOpen(false)} onSubmit={create} />
        <GlobalSearchPalette open={searchOpen} onOpenChange={setSearchOpen} />
      </div>
    </div>
  );
}
