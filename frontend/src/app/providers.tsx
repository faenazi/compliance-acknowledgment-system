"use client";

import type { ReactNode } from "react";
import { QueryProvider } from "@/lib/query/QueryProvider";
import { SessionProvider } from "@/lib/auth/SessionProvider";

/**
 * Client-side providers that wrap the whole app.
 * Ordered so that React Query is available to the SessionProvider.
 */
export function Providers({ children }: { children: ReactNode }) {
  return (
    <QueryProvider>
      <SessionProvider>{children}</SessionProvider>
    </QueryProvider>
  );
}
