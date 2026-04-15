import type { ReactNode } from "react";

/**
 * Minimal app shell for Sprint 0.
 * Role-aware navigation and authenticated context are added in Sprint 1.
 */
export function AppShell({ children }: { children: ReactNode }) {
  return (
    <div className="min-h-screen bg-[var(--color-surface-page)] text-[var(--color-text-primary)]">
      <header className="border-b border-[var(--color-border-default)] bg-white">
        <div className="mx-auto flex h-16 max-w-[1280px] items-center justify-between px-6">
          <div className="flex items-center gap-3">
            <div
              className="h-8 w-8 rounded-md"
              style={{ backgroundColor: "var(--color-brand-primary)" }}
              aria-hidden
            />
            <span className="text-base font-semibold">منصة الإقرارات المؤسسية</span>
          </div>
          <span className="text-xs text-[var(--color-text-tertiary)]">EAP</span>
        </div>
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
