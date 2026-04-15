"use client";

import type { ReactNode } from "react";
import { QueryProvider } from "@/lib/query/QueryProvider";

/**
 * Client-side providers that wrap the whole app.
 * Kept thin and composable — add providers here when introduced
 * (auth session, feature flags, i18n, etc.).
 */
export function Providers({ children }: { children: ReactNode }) {
  return <QueryProvider>{children}</QueryProvider>;
}
