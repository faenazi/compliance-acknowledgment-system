import type { ReactNode } from "react";
import { AuthGuard } from "@/lib/auth/AuthGuard";
import { AuthenticatedAppShell } from "@/components/layout/AuthenticatedAppShell";
import { USER_PORTAL_NAV } from "@/components/layout/PortalNav";

/**
 * Layout for the employee-facing portal. Any authenticated user may access
 * these routes; role-based restrictions (admin navigation, etc.) are applied
 * at the link/section level in <see cref="PortalNav"/>.
 */
export default function UserPortalLayout({ children }: { children: ReactNode }) {
  return (
    <AuthGuard>
      <AuthenticatedAppShell nav={USER_PORTAL_NAV} portalLabel="بوابة الموظف">
        {children}
      </AuthenticatedAppShell>
    </AuthGuard>
  );
}
