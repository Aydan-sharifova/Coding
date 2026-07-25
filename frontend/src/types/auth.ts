export type Role = "SuperAdmin" | "Admin" | "User" | "Developer" | "Guest";

export interface AuthenticatedUser {
  id: string;
  firstName: string;
  lastName: string;
  userName: string;
  email: string;
  isEmailVerified: boolean;
  roles: Role[];
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

export interface RegisterPayload extends LoginPayload {
  firstName: string;
  lastName: string;
  userName: string;
}
