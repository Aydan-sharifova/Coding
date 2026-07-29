export type Role = "SuperAdmin" | "Admin" | "User" | "Developer" | "Guest";

export interface AuthenticatedUser {
  id: string;
  firstName: string;
  lastName: string;
  userName: string;
  email: string;
  isEmailVerified: boolean;
  roles: Role[];
  isDemo: boolean;
  demoRole: DemoRole | null;
  demoProjectId: string | null;
}

export interface AuthResponse {
  accessToken: string;
  accessTokenExpiresAt: string;
  user: AuthenticatedUser;
}

export interface LoginPayload {
  email: string;
  password: string;
}

export type DemoRole = "Owner" | "Admin" | "Member";

export interface DemoLoginPayload {
  role: DemoRole;
}

export interface RegisterPayload extends LoginPayload {
  firstName: string;
  lastName: string;
  userName: string;
}
