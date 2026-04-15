"use client";

import { Suspense, useEffect } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";
import { useSession } from "@/lib/auth/SessionProvider";
import { ADMIN_ROLES, hasAnyRole } from "@/lib/auth/roles";
import { LoginForm } from "./LoginForm";

/**
 * LDAP / Active Directory login page. Authenticated users are redirected
 * to the role-appropriate landing area automatically so they don't get
 * stuck on the login form after their session is restored.
 */
export default function LoginPage() {
  return (
    <div className="min-h-screen bg-[var(--color-surface-subtle)]">
      <div className="mx-auto flex min-h-screen max-w-[480px] flex-col justify-center px-6 py-12">
        <div className="mb-6 flex items-center gap-3">
          <div
            aria-hidden
            className="h-10 w-10 rounded-md"
            style={{ backgroundColor: "var(--color-brand-primary)" }}
          />
          <div>
            <div className="text-sm text-[var(--color-text-tertiary)]">
              The Environment Fund
            </div>
            <div className="text-base font-semibold leading-tight">
              منصة الإقرارات المؤسسية
            </div>
          </div>
        </div>

        <Card>
          <CardHeader>
            <CardTitle>تسجيل الدخول</CardTitle>
          </CardHeader>
          <CardBody>
            <p className="text-sm text-[var(--color-text-secondary)]">
              استخدم بيانات الحساب المؤسسي (Active Directory) للدخول إلى المنصة.
            </p>
            <Suspense fallback={null}>
              <LoginPageBody />
            </Suspense>
          </CardBody>
        </Card>
      </div>
    </div>
  );
}

/**
 * Client body wrapped in Suspense so that <code>useSearchParams</code>
 * participates in the streaming render contract required by Next.js.
 */
function LoginPageBody() {
  const router = useRouter();
  const params = useSearchParams();
  const { user, isAuthenticated, isLoading } = useSession();

  useEffect(() => {
    if (isLoading || !isAuthenticated || !user) return;

    const next = params.get("next");
    if (next && next.startsWith("/")) {
      router.replace(next);
      return;
    }

    const landing = hasAnyRole(user.roles, ADMIN_ROLES) ? "/admin/dashboard" : "/dashboard";
    router.replace(landing);
  }, [isLoading, isAuthenticated, user, router, params]);

  return <LoginForm />;
}
