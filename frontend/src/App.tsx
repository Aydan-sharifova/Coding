import { Navigate, Route, Routes } from "react-router-dom";
import { lazy, Suspense } from "react";
import { AuthLayout } from "./layouts/AuthLayout";
import { useAuth } from "./hooks/useAuth";
import { RouteErrorBoundary } from "./components/RouteErrorBoundary";
import { RequireSystemRole } from "./components/RequireSystemRole";
import { PageSkeleton } from "./components/AsyncState";

const LoginPage = lazy(() => import("./pages/LoginPage").then((module) => ({ default: module.LoginPage })));
const RegisterPage = lazy(() => import("./pages/RegisterPage").then((module) => ({ default: module.RegisterPage })));
const ErrorPage = lazy(() => import("./pages/ErrorPage").then((module) => ({ default: module.ErrorPage })));
const DashboardLayout = lazy(() => import("./layouts/DashboardLayout").then((module) => ({ default: module.DashboardLayout })));
const DashboardPage = lazy(() => import("./pages/DashboardPage").then((module) => ({ default: module.DashboardPage })));
const ProjectsPage = lazy(() => import("./pages/ProjectsPage").then((module) => ({ default: module.ProjectsPage })));
const ProjectSettingsPage = lazy(() => import("./pages/ProjectSettingsPage").then((module) => ({ default: module.ProjectSettingsPage })));
const InvitationPage = lazy(() => import("./pages/InvitationPage").then((module) => ({ default: module.InvitationPage })));
const FileExplorerPage = lazy(() => import("./pages/FileExplorerPage").then((module) => ({ default: module.FileExplorerPage })));
const ChatPage = lazy(() => import("./pages/ChatPage").then((module) => ({ default: module.ChatPage })));
const NotificationCenterPage = lazy(() => import("./pages/NotificationCenterPage").then((module) => ({ default: module.NotificationCenterPage })));
const KanbanPage = lazy(() => import("./pages/KanbanPage").then((module) => ({ default: module.KanbanPage })));
const AdminActivityPage = lazy(() => import("./pages/AdminActivityPage").then((module) => ({ default: module.AdminActivityPage })));
const SettingsPage = lazy(() => import("./pages/SettingsPage").then((module) => ({ default: module.SettingsPage })));
const HelpCenterPage = lazy(() => import("./pages/HelpCenterPage").then((module) => ({ default: module.HelpCenterPage })));
const TeamPage = lazy(() => import("./pages/TeamPage").then((module) => ({ default: module.TeamPage })));
const AnalyticsPage = lazy(() => import("./pages/AnalyticsPage").then((module) => ({ default: module.AnalyticsPage })));
const AdminPage = lazy(() => import("./pages/AdminPage").then((module) => ({ default: module.AdminPage })));

function ProtectedDashboard() {
  const { session, isInitializing } = useAuth();
  if (isInitializing) return <PageSkeleton />;
  return session ? <RouteErrorBoundary><Suspense fallback={<div className="route-loader" role="status">Loading workspace…</div>}><DashboardLayout /></Suspense></RouteErrorBoundary> : <Navigate to="/login" replace />;
}

export default function App() {
  return (
    <Routes>
      <Route element={<AuthLayout />}>
        <Route path="/login" element={<Suspense fallback={<PageSkeleton />}><LoginPage /></Suspense>} />
        <Route path="/register" element={<Suspense fallback={<PageSkeleton />}><RegisterPage /></Suspense>} />
        <Route path="/401" element={<Suspense fallback={<PageSkeleton />}><ErrorPage code={401} /></Suspense>} />
      </Route>
      <Route element={<ProtectedDashboard />}>
        <Route path="/dashboard" element={<Suspense fallback={<div className="route-loader" role="status">Loading dashboard…</div>}><DashboardPage /></Suspense>} />
        <Route path="/projects" element={<Suspense fallback={<div className="route-loader" role="status">Loading projects…</div>}><ProjectsPage /></Suspense>} />
        <Route path="/projects/:projectId/settings" element={<Suspense fallback={<div className="route-loader" role="status">Loading project…</div>}><ProjectSettingsPage /></Suspense>} />
        <Route path="/projects/:projectId/workspace" element={<Suspense fallback={<div className="route-loader" role="status">Loading workspace…</div>}><FileExplorerPage /></Suspense>} />
        <Route path="/projects/:projectId/board" element={<Suspense fallback={<div className="route-loader" role="status">Loading board…</div>}><KanbanPage /></Suspense>} />
        <Route element={<RequireSystemRole roles={["SuperAdmin", "Admin"]} />}>
          <Route path="/admin" element={<Suspense fallback={<div className="route-loader" role="status">Loading administration…</div>}><AdminPage /></Suspense>} />
          <Route path="/admin/activity" element={<Suspense fallback={<div className="route-loader" role="status">Loading activity…</div>}><AdminActivityPage /></Suspense>} />
        </Route>
        <Route path="/chat" element={<Suspense fallback={<div className="route-loader" role="status">Loading chat…</div>}><ChatPage /></Suspense>} />
        <Route path="/notifications" element={<Suspense fallback={<div className="route-loader" role="status">Loading notifications…</div>}><NotificationCenterPage /></Suspense>} />
        <Route path="/settings" element={<Suspense fallback={<div className="route-loader" role="status">Loading settings…</div>}><SettingsPage /></Suspense>} />
        <Route path="/help" element={<Suspense fallback={<div className="route-loader" role="status">Loading help center…</div>}><HelpCenterPage /></Suspense>} />
        <Route path="/team" element={<Suspense fallback={<div className="route-loader" role="status">Loading team…</div>}><TeamPage /></Suspense>} />
        <Route path="/analytics" element={<Suspense fallback={<div className="route-loader" role="status">Loading analytics…</div>}><AnalyticsPage /></Suspense>} />
        <Route path="/invitations/:token" element={<Suspense fallback={<div className="route-loader" role="status">Loading invitation…</div>}><InvitationPage /></Suspense>} />
      </Route>
      <Route path="/403" element={<Suspense fallback={<PageSkeleton />}><ErrorPage code={403} /></Suspense>} />
      <Route path="/500" element={<Suspense fallback={<PageSkeleton />}><ErrorPage code={500} /></Suspense>} />
      <Route path="*" element={<Suspense fallback={<PageSkeleton />}><ErrorPage code={404} /></Suspense>} />
    </Routes>
  );
}
