import { Navigate, Route, Routes } from "react-router-dom";
import { lazy, Suspense } from "react";
import { AuthLayout } from "./layouts/AuthLayout";
import { LoginPage } from "./pages/LoginPage";
import { RegisterPage } from "./pages/RegisterPage";
import { useAuth } from "./hooks/useAuth";

const DashboardLayout = lazy(() => import("./layouts/DashboardLayout").then((module) => ({ default: module.DashboardLayout })));
const DashboardPage = lazy(() => import("./pages/DashboardPage").then((module) => ({ default: module.DashboardPage })));
const ProjectsPage = lazy(() => import("./pages/ProjectsPage").then((module) => ({ default: module.ProjectsPage })));
const ProjectSettingsPage = lazy(() => import("./pages/ProjectSettingsPage").then((module) => ({ default: module.ProjectSettingsPage })));
const InvitationPage = lazy(() => import("./pages/InvitationPage").then((module) => ({ default: module.InvitationPage })));
const FileExplorerPage = lazy(() => import("./pages/FileExplorerPage").then((module) => ({ default: module.FileExplorerPage })));
const ChatPage = lazy(() => import("./pages/ChatPage").then((module) => ({ default: module.ChatPage })));
const NotificationCenterPage = lazy(() => import("./pages/NotificationCenterPage").then((module) => ({ default: module.NotificationCenterPage })));

function ProtectedDashboard() {
  const { session, isInitializing } = useAuth();
  if (isInitializing) return <div className="route-loader" role="status">Restoring your session…</div>;
  return session ? <Suspense fallback={<div className="route-loader" role="status">Loading workspace…</div>}><DashboardLayout /></Suspense> : <Navigate to="/login" replace />;
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
        <Route path="/chat" element={<Suspense fallback={<div className="route-loader" role="status">Loading chat…</div>}><ChatPage /></Suspense>} />
        <Route path="/notifications" element={<Suspense fallback={<div className="route-loader" role="status">Loading notifications…</div>}><NotificationCenterPage /></Suspense>} />
        <Route path="/invitations/:token" element={<Suspense fallback={<div className="route-loader" role="status">Loading invitation…</div>}><InvitationPage /></Suspense>} />
      </Route>
      <Route path="*" element={<Navigate to="/login" replace />} />
    </Routes>
  );
}
