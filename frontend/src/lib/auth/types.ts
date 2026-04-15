/**
 * Shared authentication/session types used by the frontend.
 * Aligned with the DTOs exposed by Eap.Api (AuthController, GetCurrentUserQuery).
 */

export type ScopeType = 0 | 1 | 2; // 0 = Global, 1 = Department, 2 = OwnedContent

export interface SessionScope {
  type: ScopeType;
  reference: string;
  roleName: string;
}

export interface SessionUser {
  userId: string;
  username: string;
  displayName: string;
  email: string;
  department: string | null;
  jobTitle: string | null;
  roles: string[];
  scopes: SessionScope[];
}

export interface LoginResult {
  userId: string;
  username: string;
  displayName: string;
  email: string;
  department: string | null;
  jobTitle: string | null;
  roles: string[];
}
