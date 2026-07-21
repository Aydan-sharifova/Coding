import type { AuthResponse, LoginPayload, RegisterPayload } from "../types/auth";
import { apiClient } from "./apiClient";
import { tokenStore } from "./tokenStore";

async function establishSession(request: Promise<AuthResponse>) {
  const session = await request;
  tokenStore.set(session.accessToken);
  return session;
}

export const authService = {
  login: (payload: LoginPayload) => establishSession(apiClient.post<AuthResponse>("/auth/login", payload, { retryOnUnauthorized: false })),
  register: (payload: RegisterPayload) => establishSession(apiClient.post<AuthResponse>("/auth/register", payload, { retryOnUnauthorized: false })),
  refresh: () => apiClient.refreshSession(),
  logout: async () => {
    try {
      await apiClient.post<void>("/auth/logout", undefined, { retryOnUnauthorized: false });
    } finally {
      tokenStore.clear();
    }
  },
};
