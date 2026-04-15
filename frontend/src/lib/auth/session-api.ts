import { apiClient } from "@/lib/api/client";
import type { LoginResult, SessionUser } from "./types";

/**
 * Thin API-adapter layer for authentication endpoints exposed by Eap.Api.
 * Keeps request/response handling out of React components and hooks.
 */

export async function login(username: string, password: string): Promise<LoginResult> {
  const { data } = await apiClient.post<LoginResult>("/api/auth/login", {
    username,
    password,
  });
  return data;
}

export async function logout(): Promise<void> {
  await apiClient.post("/api/auth/logout");
}

export async function fetchCurrentUser(): Promise<SessionUser | null> {
  try {
    const { data } = await apiClient.get<SessionUser>("/api/auth/me");
    return data;
  } catch (error) {
    // 401 is expected when no session exists.
    if (isUnauthorized(error)) {
      return null;
    }
    throw error;
  }
}

function isUnauthorized(error: unknown): boolean {
  if (typeof error === "object" && error !== null && "status" in error) {
    return (error as { status: number }).status === 401;
  }
  return false;
}
