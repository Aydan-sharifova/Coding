import { createContext, useCallback, useMemo, useState, type PropsWithChildren } from "react";
import { authService } from "../services/authService";
import type { AuthResponse, LoginPayload, RegisterPayload } from "../types/auth";

interface AuthContextValue {
  session: AuthResponse | null;
  login: (payload: LoginPayload) => Promise<void>;
  register: (payload: RegisterPayload) => Promise<void>;
  logout: () => Promise<void>;
}

export const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: PropsWithChildren) {
  const [session, setSession] = useState<AuthResponse | null>(null);

  const login = useCallback(async (payload: LoginPayload) => {
    setSession(await authService.login(payload));
  }, []);

  const register = useCallback(async (payload: RegisterPayload) => {
    setSession(await authService.register(payload));
  }, []);

  const logout = useCallback(async () => {
    await authService.logout();
    setSession(null);
  }, []);

  const value = useMemo(() => ({ session, login, register, logout }), [session, login, register, logout]);
  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
