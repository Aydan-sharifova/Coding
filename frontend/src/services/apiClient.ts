import type { AuthResponse } from "../types/auth";
import { tokenStore } from "./tokenStore";

const API_URL = import.meta.env.VITE_API_URL ?? "/api";
let refreshRequest: Promise<AuthResponse> | null = null;

export class ApiError extends Error {
  constructor(message: string, public readonly status: number) {
    super(message);
    this.name = "ApiError";
  }
}

async function getError(response: Response): Promise<ApiError> {
  const problem = await response.json().catch(() => null) as {
    title?: string;
    detail?: string;
    errors?: Record<string, string[]>;
  } | null;
  const validationError = problem?.errors
    ? Object.values(problem.errors).flat()[0]
    : undefined;
  const gatewayMessage = [502, 503, 504].includes(response.status)
    ? "The API is unavailable. Make sure the backend is running on port 5192."
    : undefined;
  return new ApiError(validationError ?? problem?.detail ?? problem?.title ?? gatewayMessage ?? "We couldn't complete your request.", response.status);
}

async function refreshSession(): Promise<AuthResponse> {
  if (!refreshRequest) {
    refreshRequest = fetch(`${API_URL}/auth/refresh`, {
      method: "POST",
      credentials: "include",
    }).then(async (response) => {
      if (!response.ok) throw await getError(response);
      const session = await response.json() as AuthResponse;
      tokenStore.set(session.accessToken);
      return session;
    }).finally(() => { refreshRequest = null; });
  }
  return refreshRequest;
}

interface RequestOptions extends RequestInit {
  retryOnUnauthorized?: boolean;
}

async function request<TResponse>(path: string, options: RequestOptions = {}): Promise<TResponse> {
  const { retryOnUnauthorized = true, headers, ...requestOptions } = options;
  const token = tokenStore.get();
  let response: Response;
  try {
    response = await fetch(`${API_URL}${path}`, {
      ...requestOptions,
      credentials: "include",
      headers: {
        ...(requestOptions.body ? { "Content-Type": "application/json" } : {}),
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
        ...headers,
      },
    });
  } catch {
    throw new ApiError("Cannot connect to the API. Make sure the backend is running on port 5192.", 0);
  }

  if (response.status === 401 && retryOnUnauthorized && !path.startsWith("/auth/")) {
    await refreshSession();
    return request<TResponse>(path, { ...options, retryOnUnauthorized: false });
  }
  if (!response.ok) throw await getError(response);
  if (response.status === 204) return undefined as TResponse;
  return await response.json() as TResponse;
}

export const apiClient = {
  get: <TResponse>(path: string) => request<TResponse>(path),
  post: <TResponse>(path: string, body?: unknown, options?: RequestOptions) => request<TResponse>(path, {
    ...options,
    method: "POST",
    body: body === undefined ? undefined : JSON.stringify(body),
  }),
  put: <TResponse>(path: string, body: unknown) => request<TResponse>(path, { method: "PUT", body: JSON.stringify(body) }),
  delete: <TResponse>(path: string) => request<TResponse>(path, { method: "DELETE" }),
  refreshSession,
};
