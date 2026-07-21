import type { AuthResponse, LoginPayload, RegisterPayload } from "../types/auth";

const API_URL = import.meta.env.VITE_API_URL ?? "http://localhost:8080/api";

class ApiError extends Error {
  constructor(
    message: string,
    public readonly status: number,
  ) {
    super(message);
    this.name = "ApiError";
  }
}

async function post<TResponse>(path: string, body?: unknown): Promise<TResponse> {
  const response = await fetch(`${API_URL}${path}`, {
    method: "POST",
    credentials: "include",
    headers: body ? { "Content-Type": "application/json" } : undefined,
    body: body ? JSON.stringify(body) : undefined,
  });

  if (!response.ok) {
    const problem = await response.json().catch(() => null) as { title?: string } | null;
    throw new ApiError(problem?.title ?? "We couldn't complete your request.", response.status);
  }

  return response.status === 204
    ? undefined as TResponse
    : await response.json() as TResponse;
}

export const authService = {
  login: (payload: LoginPayload) => post<AuthResponse>("/auth/login", payload),
  register: (payload: RegisterPayload) => post<AuthResponse>("/auth/register", payload),
  refresh: () => post<AuthResponse>("/auth/refresh"),
  logout: () => post<void>("/auth/logout"),
};
