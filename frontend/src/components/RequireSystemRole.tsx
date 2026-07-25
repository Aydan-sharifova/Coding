import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";

export function RequireSystemRole({ roles }: { roles: string[] }) {
  const { session } = useAuth();
  return session?.user.roles.some((role) => roles.includes(role))
    ? <Outlet />
    : <Navigate to="/dashboard" replace />;
}
