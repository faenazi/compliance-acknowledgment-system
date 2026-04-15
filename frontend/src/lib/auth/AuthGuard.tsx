"use client";

import { useEffect, type ReactNode } from "react";
import { useRouter, usePathname } from "next/navigation";
import { useSession } from "./SessionProvider";
import { hasAnyRole } from "./roles";

interface AuthGuardProps {
  children: ReactNode;
  /** Optional role allow-list. When set, users without at least one matching role are redirected. */
  requireRoles?: readonly string[];
  /** Path used for redirects when the user is not authenticated. */
  loginPath?: string;
  /** Path used when a role requirement is not met. */
  forbiddenPath?: string;
}

/**
 * Client-side route guard. Verifies that the current session meets the
 * configured requirements and redirects otherwise. Server-side authorization
 * is enforced by the backend — this guard optimises UX, not security.
 */
export function AuthGuard({
  children,
  requireRoles,
  loginPath = "/login",
  forbiddenPath = "/",
}: AuthGuardProps) {
  const router = useRouter();
  const pathname = usePathname();
  const { user, isLoading, isAuthenticated } = useSession();

  const roleOk =
    !requireRoles || (user ? hasAnyRole(user.roles, requireRoles) : false);

  useEffect(() => {
    if (isLoading) return;

    if (!isAuthenticated) {
      const next = pathname && pathname !== loginPath ? `?next=${encodeURIComponent(pathname)}` : "";
      router.replace(`${loginPath}${next}`);
      return;
    }

    if (!roleOk) {
      router.replace(forbiddenPath);
    }
  }, [isLoading, isAuthenticated, roleOk, router, pathname, loginPath, forbiddenPath]);

  if (isLoading || !isAuthenticated || !roleOk) {
    return (
      <div
        role="status"
        aria-live="polite"
        className="flex min-h-[50vh] items-center justify-center text-sm text-[var(--color-text-tertiary)]"
      >
        جاري التحقق من الجلسة…
      </div>
    );
  }

  return <>{children}</>;
}
