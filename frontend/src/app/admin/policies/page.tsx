"use client";

import Link from "next/link";
import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";
import { PolicyStatusBadge } from "@/components/policies/PolicyStatusBadge";
import { usePolicies } from "@/lib/policies/hooks";
import { PolicyStatus } from "@/lib/policies/types";
import { formatDate } from "@/lib/admin/labels";
import { useSession } from "@/lib/auth/SessionProvider";
import { Roles, hasAnyRole } from "@/lib/auth/roles";

const PAGE_SIZE = 20;

/**
 * Admin portal policies index (Sprint 7 refinement).
 * Adds "Last Updated" column, "Owner Department" filter, and
 * improves overall table readability per admin-portal-pages.md §5.
 */
export default function AdminPoliciesPage() {
  const { user } = useSession();
  const canAuthor = hasAnyRole(user?.roles ?? [], [Roles.PolicyManager, Roles.SystemAdministrator]);

  const [page, setPage] = useState(1);
  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState<PolicyStatus | "">("");
  const [deptFilter, setDeptFilter] = useState("");

  const { data, isLoading, isError, error, refetch } = usePolicies({
    page,
    pageSize: PAGE_SIZE,
    search: search || undefined,
    status: statusFilter === "" ? undefined : statusFilter,
    ownerDepartment: deptFilter || undefined,
  });

  const items = data?.items ?? [];
  const totalPages = data?.totalPages ?? 0;

  return (
    <>
      <section className="mb-6 flex flex-wrap items-start justify-between gap-4">
        <div>
          <h1 className="text-3xl font-bold">السياسات</h1>
          <p className="mt-2 text-[var(--color-text-secondary)]">
            إدارة السياسات والنسخ والمستندات المرفقة. يخضع النشر لقاعدة نسخة واحدة منشورة لكل سياسة.
          </p>
        </div>
        {canAuthor ? (
          <Link href="/admin/policies/new">
            <Button>+ سياسة جديدة</Button>
          </Link>
        ) : null}
      </section>

      <Card>
        <CardHeader>
          <CardTitle>قائمة السياسات</CardTitle>
        </CardHeader>
        <CardBody>
          {/* Filter bar */}
          <div className="mb-5 flex flex-wrap items-end gap-3">
            <div className="flex-1 min-w-[220px]">
              <label htmlFor="policy-search" className="block text-xs font-medium text-[var(--color-text-tertiary)]">
                بحث بالعنوان أو الرمز
              </label>
              <input
                id="policy-search"
                type="search"
                value={search}
                onChange={(e) => {
                  setPage(1);
                  setSearch(e.target.value);
                }}
                className="mt-1 block h-10 w-full rounded-[10px] border border-[var(--color-border-strong)] bg-white px-3 text-sm outline-none focus:border-[var(--color-brand-primary)]"
              />
            </div>
            <div>
              <label htmlFor="policy-status" className="block text-xs font-medium text-[var(--color-text-tertiary)]">
                الحالة
              </label>
              <select
                id="policy-status"
                value={statusFilter}
                onChange={(e) => {
                  setPage(1);
                  setStatusFilter(e.target.value === "" ? "" : Number(e.target.value) as PolicyStatus);
                }}
                className="mt-1 h-10 rounded-[10px] border border-[var(--color-border-strong)] bg-white px-3 text-sm"
              >
                <option value="">الكل</option>
                <option value={PolicyStatus.Draft}>مسودة</option>
                <option value={PolicyStatus.Published}>منشورة</option>
                <option value={PolicyStatus.Archived}>مؤرشفة</option>
              </select>
            </div>
            <div className="min-w-[160px]">
              <label htmlFor="policy-dept" className="block text-xs font-medium text-[var(--color-text-tertiary)]">
                الإدارة المالكة
              </label>
              <input
                id="policy-dept"
                type="text"
                value={deptFilter}
                placeholder="الكل"
                onChange={(e) => {
                  setPage(1);
                  setDeptFilter(e.target.value);
                }}
                className="mt-1 block h-10 w-full rounded-[10px] border border-[var(--color-border-strong)] bg-white px-3 text-sm outline-none focus:border-[var(--color-brand-primary)]"
              />
            </div>
          </div>

          {isLoading ? (
            <p className="text-sm text-[var(--color-text-tertiary)]">جاري التحميل…</p>
          ) : isError ? (
            <div className="space-y-3">
              <p className="text-sm text-red-700">تعذّر تحميل السياسات: {error?.title ?? "خطأ غير معروف"}</p>
              <Button variant="secondary" onClick={() => refetch()}>
                إعادة المحاولة
              </Button>
            </div>
          ) : items.length === 0 ? (
            <p className="text-sm text-[var(--color-text-tertiary)]">
              لا توجد سياسات مطابقة للبحث.
            </p>
          ) : (
            <>
              <div className="overflow-x-auto">
                <table className="w-full text-right text-sm">
                  <thead className="border-b border-[var(--color-border-default)] text-xs text-[var(--color-text-tertiary)]">
                    <tr>
                      <th className="py-2 pe-4 font-medium">الرمز</th>
                      <th className="py-2 pe-4 font-medium">العنوان</th>
                      <th className="py-2 pe-4 font-medium">الإدارة المالكة</th>
                      <th className="py-2 pe-4 font-medium">الحالة</th>
                      <th className="py-2 pe-4 font-medium">النسخة الحالية</th>
                      <th className="py-2 pe-4 font-medium">عدد النسخ</th>
                      <th className="py-2 pe-4 font-medium">آخر تحديث</th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-[var(--color-border-default)]">
                    {items.map((p) => (
                      <tr key={p.id} className="hover:bg-[var(--color-surface-subtle)]">
                        <td className="py-3 pe-4 font-mono text-xs" dir="ltr">{p.policyCode}</td>
                        <td className="py-3 pe-4">
                          <Link
                            href={`/admin/policies/${p.id}`}
                            className="font-medium text-[var(--color-text-link)] hover:underline"
                          >
                            {p.title}
                          </Link>
                        </td>
                        <td className="py-3 pe-4">{p.ownerDepartment}</td>
                        <td className="py-3 pe-4"><PolicyStatusBadge status={p.status} /></td>
                        <td className="py-3 pe-4">
                          {p.currentVersionNumber ? `v${p.currentVersionNumber}` : "—"}
                        </td>
                        <td className="py-3 pe-4">{p.versionsCount}</td>
                        <td className="py-3 pe-4 text-xs text-[var(--color-text-tertiary)]">
                          {formatDate(p.updatedAtUtc ?? p.createdAtUtc)}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>

              {totalPages > 1 ? (
                <div className="mt-4 flex items-center justify-between text-sm">
                  <span className="text-[var(--color-text-tertiary)]">
                    صفحة {page} من {totalPages} — إجمالي {data?.totalCount ?? 0} سياسة
                  </span>
                  <div className="flex items-center gap-2">
                    <Button
                      variant="secondary"
                      size="sm"
                      disabled={!data?.hasPrevious}
                      onClick={() => setPage((p) => Math.max(1, p - 1))}
                    >
                      السابق
                    </Button>
                    <Button
                      variant="secondary"
                      size="sm"
                      disabled={!data?.hasNext}
                      onClick={() => setPage((p) => p + 1)}
                    >
                      التالي
                    </Button>
                  </div>
                </div>
              ) : null}
            </>
          )}
        </CardBody>
      </Card>
    </>
  );
}
