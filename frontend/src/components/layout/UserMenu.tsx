"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { LogOut, User } from "lucide-react";
import { useSession } from "@/lib/auth/SessionProvider";

/**
 * Compact profile summary with a sign-out action. Sits in the authenticated
 * app shell header. Kept intentionally small for the MVP.
 */
export function UserMenu() {
  const { user, logout } = useSession();
  const router = useRouter();
  const [isSigningOut, setIsSigningOut] = useState(false);

  if (!user) return null;

  const handleLogout = async () => {
    try {
      setIsSigningOut(true);
      await logout();
      router.replace("/login");
    } finally {
      setIsSigningOut(false);
    }
  };

  return (
    <div className="flex items-center gap-3">
      <div className="hidden text-end sm:block">
        <div className="text-sm font-semibold text-[var(--color-text-primary)]">
          {user.displayName}
        </div>
        <div className="text-xs text-[var(--color-text-tertiary)]">
          {user.department ?? user.jobTitle ?? user.email}
        </div>
      </div>
      <div
        aria-hidden
        className="flex h-9 w-9 items-center justify-center rounded-full"
        style={{
          backgroundColor: "var(--color-surface-subtle)",
          color: "var(--color-brand-primary)",
        }}
      >
        <User size={18} />
      </div>
      <button
        type="button"
        onClick={handleLogout}
        disabled={isSigningOut}
        className="inline-flex items-center gap-1 rounded-md border border-[var(--color-border-default)] px-3 py-1.5 text-xs font-medium text-[var(--color-text-secondary)] transition-colors hover:bg-[var(--color-surface-subtle)] disabled:opacity-50"
      >
        <LogOut size={14} />
        تسجيل الخروج
      </button>
    </div>
  );
}
