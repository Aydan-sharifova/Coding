import { Outlet } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";
import { PermissionDenied } from "./AsyncState";

export function RequireSystemRole({ roles }: { roles: string[] }) {
  const { session } = useAuth();
  return session?.user.roles.some((role) => roles.includes(role))
    ? <Outlet />
    : <PermissionDenied onBack={()=>history.back()} />;
}
