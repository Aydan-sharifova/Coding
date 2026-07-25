import { Navigate, Route, Routes } from "react-router-dom";
import { lazy, Suspense } from "react";
import { AuthLayout } from "./layouts/AuthLayout";
import { LoginPage } from "./pages/LoginPage";
import { RegisterPage } from "./pages/RegisterPage";
import { useAuth } from "./hooks/useAuth";
import { RouteErrorBoundary } from "./components/RouteErrorBoundary";

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

function ProtectedDashboard() {
  const { session, isInitializing } = useAuth();
  if (isInitializing) return <div className="route-loader" role="status">Restoring your session…</div>;
  return session ? <RouteErrorBoundary><Suspense fallback={<div className="route-loader" role="status">Loading workspace…</div>}><DashboardLayout /></Suspense></RouteErrorBoundary> : <Navigate to="/login" replace />;
}

export default function App() {
  return (
    <Routes>
      <Route element={<AuthLayout />}>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
      </Route>
      <Route element={<ProtectedDashboard />}>
        <Route path="/dashboard" element={<Suspense fallback={<div className="route-loader" role="status">Loading dashboard…</div>}><DashboardPage /></Suspense>} />
        <Route path="/projects" element={<Suspense fallback={<div className="route-loader" role="status">Loading projects…</div>}><ProjectsPage /></Suspense>} />
        <Route path="/projects/:projectId/settings" element={<Suspense fallback={<div className="route-loader" role="status">Loading project…</div>}><ProjectSettingsPage /></Suspense>} />
        <Route path="/projects/:projectId/workspace" element={<Suspense fallback={<div className="route-loader" role="status">Loading workspace…</div>}><FileExplorerPage /></Suspense>} />
        <Route path="/projects/:projectId/board" element={<Suspense fallback={<div className="route-loader" role="status">Loading board…</div>}><KanbanPage /></Suspense>} />
        <Route path="/admin/activity" element={<Suspense fallback={<div className="route-loader" role="status">Loading activity…</div>}><AdminActivityPage /></Suspense>} />
        <Route path="/chat" element={<Suspense fallback={<div className="route-loader" role="status">Loading chat…</div>}><ChatPage /></Suspense>} />
        <Route path="/notifications" element={<Suspense fallback={<div className="route-loader" role="status">Loading notifications…</div>}><NotificationCenterPage /></Suspense>} />
        <Route path="/settings" element={<Suspense fallback={<div className="route-loader" role="status">Loading settings…</div>}><SettingsPage /></Suspense>} />
        <Route path="/help" element={<Suspense fallback={<div className="route-loader" role="status">Loading help center…</div>}><HelpCenterPage /></Suspense>} />
        <Route path="/team" element={<Suspense fallback={<div className="route-loader" role="status">Loading team…</div>}><TeamPage /></Suspense>} />
        <Route path="/analytics" element={<Suspense fallback={<div className="route-loader" role="status">Loading analytics…</div>}><AnalyticsPage /></Suspense>} />
        <Route path="/invitations/:token" element={<Suspense fallback={<div className="route-loader" role="status">Loading invitation…</div>}><InvitationPage /></Suspense>} />
      </Route>
      <Route path="*" element={<Navigate to="/login" replace />} />
    </Routes>
  );
}
