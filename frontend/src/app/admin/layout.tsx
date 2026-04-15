import type { ReactNode } from "react";
import { AuthGuard } from "@/lib/auth/AuthGuard";
import { AuthenticatedAppShell } from "@/components/layout/AuthenticatedAppShell";
import { ADMIN_PORTAL_NAV } from "@/components/layout/PortalNav";
import { ADMIN_ROLES } from "@/lib/auth/roles";

/**
 * Layout for the admin portal. Requires at least one admin-capable role
 * (System Administrator, Policy Manager, Acknowledgment Manager, Publisher,
 * Compliance Viewer, or Auditor). Non-admin end-users are redirected.
 */
export default function AdminPortalLayout({ children }: { children: ReactNode }) {
  return (
    <AuthGuard requireRoles={ADMIN_ROLES} forbiddenPath="/dashboard">
      <AuthenticatedAppShell nav={ADMIN_PORTAL_NAV} portalLabel="بوابة الإدارة">
        {children}
      </AuthenticatedAppShell>
    </AuthGuard>
  );
}
