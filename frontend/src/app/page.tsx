"use client";

import { useEffect } from "react";
import { useRouter } from "next/navigation";
import { useSession } from "@/lib/auth/SessionProvider";
import { ADMIN_ROLES, hasAnyRole } from "@/lib/auth/roles";

/**
 * Root landing page. Resolves the authenticated session and redirects the
 * user to the portal that matches their role. Unauthenticated visitors are
 * sent to the login page. Kept as a single responsibility so no page content
 * leaks for callers that do not yet have a session.
 */
export default function HomePage() {
  const router = useRouter();
  const { user, isAuthenticated, isLoading } = useSession();

  useEffect(() => {
    if (isLoading) return;

    if (!isAuthenticated || !user) {
      router.replace("/login");
      return;
    }

    const landing = hasAnyRole(user.roles, ADMIN_ROLES) ? "/admin/dashboard" : "/dashboard";
    router.replace(landing);
  }, [isLoading, isAuthenticated, user, router]);

  return (
    <div
      role="status"
      aria-live="polite"
      className="flex min-h-screen items-center justify-center text-sm text-[var(--color-text-tertiary)]"
    >
      جاري التحميل…
    </div>
  );
}
