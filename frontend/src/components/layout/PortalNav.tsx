"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { cn } from "@/lib/utils/cn";
import { useSession } from "@/lib/auth/SessionProvider";
import { ADMIN_ROLES, hasAnyRole } from "@/lib/auth/roles";

export interface NavItem {
  href: string;
  label: string;
  /** When set, the item is only visible if the user has at least one matching role. */
  requireRoles?: readonly string[];
}

/**
 * Role-aware primary navigation. Items are filtered against the current
 * session so the admin portal link is hidden from end-users, etc.
 */
export function PortalNav({ items }: { items: readonly NavItem[] }) {
  const pathname = usePathname();
  const { user } = useSession();
  const roles = user?.roles ?? [];

  const visible = items.filter((item) =>
    !item.requireRoles || hasAnyRole(roles, item.requireRoles),
  );

  return (
    <nav aria-label="Primary" className="flex items-center gap-1">
      {visible.map((item) => {
        const isActive =
          pathname === item.href ||
          (item.href !== "/" && pathname?.startsWith(item.href));
        return (
          <Link
            key={item.href}
            href={item.href}
            className={cn(
              "rounded-md px-3 py-1.5 text-sm font-medium transition-colors",
              isActive
                ? "bg-[var(--color-surface-subtle)] text-[var(--color-brand-primary)]"
                : "text-[var(--color-text-secondary)] hover:bg-[var(--color-surface-subtle)] hover:text-[var(--color-text-primary)]",
            )}
          >
            {item.label}
          </Link>
        );
      })}
    </nav>
  );
}

/** Shared navigation for the authenticated user portal. */
export const USER_PORTAL_NAV: readonly NavItem[] = [
  { href: "/dashboard", label: "لوحة الموظف" },
  { href: "/profile", label: "ملفي الشخصي" },
  { href: "/admin/dashboard", label: "بوابة الإدارة", requireRoles: ADMIN_ROLES },
];

/** Navigation for users operating the admin portal. */
export const ADMIN_PORTAL_NAV: readonly NavItem[] = [
  { href: "/admin/dashboard", label: "لوحة الإدارة", requireRoles: ADMIN_ROLES },
  { href: "/admin/policies", label: "السياسات", requireRoles: ADMIN_ROLES },
  { href: "/dashboard", label: "بوابة الموظف" },
];
