"use client";

import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";
import { useSession } from "@/lib/auth/SessionProvider";

/**
 * Read-only profile view. AD is the source of truth for identity data,
 * so this page is intentionally display-only (see user-portal-pages §14).
 */
export default function ProfilePage() {
  const { user } = useSession();

  if (!user) return null;

  const rows: Array<{ label: string; value: string | null }> = [
    { label: "الاسم الكامل", value: user.displayName },
    { label: "اسم المستخدم", value: user.username },
    { label: "البريد الإلكتروني", value: user.email },
    { label: "الإدارة", value: user.department },
    { label: "المسمى الوظيفي", value: user.jobTitle },
  ];

  return (
    <>
      <section className="mb-6">
        <h1 className="text-3xl font-bold">ملفي الشخصي</h1>
        <p className="mt-2 text-[var(--color-text-secondary)]">
          المعلومات التالية مصدرها Active Directory ويتم تحديثها تلقائياً.
        </p>
      </section>

      <div className="grid gap-6 md:grid-cols-2">
        <Card>
          <CardHeader>
            <CardTitle>معلومات الحساب</CardTitle>
          </CardHeader>
          <CardBody>
            <dl className="space-y-3">
              {rows.map((row) => (
                <div key={row.label} className="flex items-baseline justify-between gap-4">
                  <dt className="text-xs text-[var(--color-text-tertiary)]">{row.label}</dt>
                  <dd className="text-sm font-medium text-[var(--color-text-primary)]">
                    {row.value ?? "—"}
                  </dd>
                </div>
              ))}
            </dl>
          </CardBody>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>الأدوار والصلاحيات</CardTitle>
          </CardHeader>
          <CardBody>
            {user.roles.length === 0 ? (
              <p className="text-sm text-[var(--color-text-secondary)]">
                لا توجد أدوار مخصصة حالياً.
              </p>
            ) : (
              <ul className="flex flex-wrap gap-2">
                {user.roles.map((role) => (
                  <li
                    key={role}
                    className="rounded-full border px-3 py-1 text-xs font-semibold"
                    style={{
                      backgroundColor: "var(--color-status-published-bg)",
                      borderColor: "var(--color-status-published-border)",
                      color: "var(--color-status-published-text)",
                    }}
                  >
                    {role}
                  </li>
                ))}
              </ul>
            )}
          </CardBody>
        </Card>
      </div>
    </>
  );
}
