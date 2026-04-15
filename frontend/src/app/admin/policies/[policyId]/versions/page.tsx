"use client";

import Link from "next/link";
import { use } from "react";
import { Button } from "@/components/ui/button";
import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";
import { PolicyVersionStatusBadge } from "@/components/policies/PolicyStatusBadge";
import { usePolicy, usePolicyVersions } from "@/lib/policies/hooks";
import { PolicyStatus } from "@/lib/policies/types";
import { useSession } from "@/lib/auth/SessionProvider";
import { Roles, hasAnyRole } from "@/lib/auth/roles";

interface VersionsListPageProps {
  params: Promise<{ policyId: string }>;
}

/**
 * Full history of versions for a single policy, for managers who need a
 * dedicated screen (the policy detail page embeds a shorter card-based list).
 */
export default function PolicyVersionsListPage({ params }: VersionsListPageProps) {
  const { policyId } = use(params);
  const { user } = useSession();
  const canAuthor = hasAnyRole(user?.roles ?? [], [Roles.PolicyManager, Roles.SystemAdministrator]);

  const { data: policy } = usePolicy(policyId);
  const { data: versions, isLoading, isError, error, refetch } = usePolicyVersions(policyId);

  const isArchived = policy?.status === PolicyStatus.Archived;
  const sorted = (versions ?? []).slice().sort((a, b) => b.versionNumber - a.versionNumber);

  return (
    <>
      <section className="mb-6 flex flex-wrap items-start justify-between gap-4">
        <div>
          <div className="text-xs text-[var(--color-text-tertiary)]">
            <Link href="/admin/policies" className="hover:underline">السياسات</Link>
            <span className="mx-2">›</span>
            <Link href={`/admin/policies/${policyId}`} className="hover:underline">
              {policy?.title ?? "تفاصيل السياسة"}
            </Link>
          </div>
          <h1 className="mt-2 text-3xl font-bold">النسخ</h1>
        </div>
        {canAuthor && !isArchived ? (
          <Link href={`/admin/policies/${policyId}/versions/new`}>
            <Button>+ نسخة جديدة</Button>
          </Link>
        ) : null}
      </section>

      <Card>
        <CardHeader>
          <CardTitle>سجل النسخ</CardTitle>
        </CardHeader>
        <CardBody>
          {isLoading ? (
            <p className="text-sm text-[var(--color-text-tertiary)]">جاري التحميل…</p>
          ) : isError ? (
            <div className="space-y-3">
              <p className="text-sm text-red-700">تعذّر التحميل: {error?.title ?? "خطأ غير معروف"}</p>
              <Button variant="secondary" onClick={() => refetch()}>إعادة المحاولة</Button>
            </div>
          ) : sorted.length === 0 ? (
            <p className="text-sm text-[var(--color-text-tertiary)]">
              لم يتم إنشاء أي نسخ لهذه السياسة بعد.
            </p>
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full text-right text-sm">
                <thead className="border-b border-[var(--color-border-default)] text-xs text-[var(--color-text-tertiary)]">
                  <tr>
                    <th className="py-2 pe-4 font-medium">النسخة</th>
                    <th className="py-2 pe-4 font-medium">التسمية</th>
                    <th className="py-2 pe-4 font-medium">الحالة</th>
                    <th className="py-2 pe-4 font-medium">تاريخ النفاذ</th>
                    <th className="py-2 pe-4 font-medium">ملف مرفق</th>
                    <th className="py-2 pe-4 font-medium">النشر</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-[var(--color-border-default)]">
                  {sorted.map((v) => (
                    <tr key={v.id} className="hover:bg-[var(--color-surface-subtle)]">
                      <td className="py-3 pe-4 font-medium">
                        <Link
                          href={`/admin/policies/${policyId}/versions/${v.id}`}
                          className="text-[var(--color-text-link)] hover:underline"
                        >
                          v{v.versionNumber}
                        </Link>
                      </td>
                      <td className="py-3 pe-4">{v.versionLabel ?? "—"}</td>
                      <td className="py-3 pe-4"><PolicyVersionStatusBadge status={v.status} /></td>
                      <td className="py-3 pe-4" dir="ltr">{v.effectiveDate ?? "—"}</td>
                      <td className="py-3 pe-4">{v.hasDocument ? "نعم" : "لا"}</td>
                      <td className="py-3 pe-4" dir="ltr">
                        {v.publishedAtUtc ? new Date(v.publishedAtUtc).toLocaleDateString() : "—"}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </CardBody>
      </Card>
    </>
  );
}
