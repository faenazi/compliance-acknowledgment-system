"use client";

import type { ReactNode } from "react";
import { PortalNav, type NavItem } from "./PortalNav";
import { UserMenu } from "./UserMenu";

interface AuthenticatedAppShellProps {
  children: ReactNode;
  /** Navigation items rendered in the top bar (role-aware). */
  nav?: readonly NavItem[];
  /** Portal label displayed next to the platform wordmark. */
  portalLabel?: string;
}

/**
 * App shell used for all authenticated pages. Keeps the header (branding,
 * role-aware navigation, user menu) consistent across both the user and
 * admin portals. Styling follows the Sprint 0 design tokens.
 */
export function AuthenticatedAppShell({
  children,
  nav,
  portalLabel,
}: AuthenticatedAppShellProps) {
  return (
    <div className="min-h-screen bg-[var(--color-surface-page)] text-[var(--color-text-primary)]">
      <header className="border-b border-[var(--color-border-default)] bg-white">
        <div className="mx-auto flex h-16 max-w-[1280px] items-center justify-between gap-6 px-6">
          <div className="flex items-center gap-4">
            <div className="flex items-center gap-3">
              <div
                className="h-8 w-8 rounded-md"
                style={{ backgroundColor: "var(--color-brand-primary)" }}
                aria-hidden
              />
              <div>
                <div className="text-base font-semibold leading-tight">
                  منصة الإقرارات المؤسسية
                </div>
                {portalLabel ? (
                  <div className="text-xs text-[var(--color-text-tertiary)]">
                    {portalLabel}
                  </div>
                ) : null}
              </div>
            </div>
            {nav?.length ? (
              <div className="hidden border-s border-[var(--color-border-default)] ps-4 lg:block">
                <PortalNav items={nav} />
              </div>
            ) : null}
          </div>

          <UserMenu />
        </div>

        {nav?.length ? (
          <div className="mx-auto max-w-[1280px] px-6 pb-2 lg:hidden">
            <PortalNav items={nav} />
          </div>
        ) : null}
      </header>

      <main className="mx-auto max-w-[1280px] px-6 py-10">{children}</main>

      <footer className="border-t border-[var(--color-border-default)] py-6">
        <div className="mx-auto max-w-[1280px] px-6 text-xs text-[var(--color-text-tertiary)]">
          © The Environment Fund — Enterprise Acknowledgment Platform
        </div>
      </footer>
    </div>
  );
}
