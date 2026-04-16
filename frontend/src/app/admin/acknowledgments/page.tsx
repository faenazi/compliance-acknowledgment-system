"use client";

import Link from "next/link";
import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";
import { AcknowledgmentStatusBadge } from "@/components/acknowledgments/AcknowledgmentStatusBadge";
import { ActionTypeBadge } from "@/components/acknowledgments/ActionTypeBadge";
import { useAcknowledgmentDefinitions } from "@/lib/acknowledgments/hooks";
import {
  AcknowledgmentStatus,
  ActionType,
  RecurrenceModel,
} from "@/lib/acknowledgments/types";
import { actionTypeLabel } from "@/lib/acknowledgments/labels";
import { recurrenceModelLabel, formatDate } from "@/lib/admin/labels";
import { useSession } from "@/lib/auth/SessionProvider";
import { Roles, hasAnyRole } from "@/lib/auth/roles";

const PAGE_SIZE = 20;

/**
 * Admin portal acknowledgments index (Sprint 7 refinement).
 * Adds "Recurrence Model" column (via versions), "Owner Department" filter,
 * and "Last Updated" column per admin-portal-pages.md §9.
 */
export default function AdminAcknowledgmentsPage() {
  const { user } = useSession();
  const canAuthor = hasAnyRole(user?.roles ?? [], [
    Roles.AcknowledgmentManager,
    Roles.SystemAdministrator,
  ]);

  const [page, setPage] = useState(1);
  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState<AcknowledgmentStatus | "">("");
  const [actionFilter, setActionFilter] = useState<ActionType | "">("");
  const [deptFilter, setDeptFilter] = useState("");

  const { data, isLoading, isError, error, refetch } = useAcknowledgmentDefinitions({
    page,
    pageSize: PAGE_SIZE,
    search: search || undefined,
    status: statusFilter === "" ? undefined : statusFilter,
    actionType: actionFilter === "" ? undefined : actionFilter,
    ownerDepartment: deptFilter || undefined,
  });

  const items = data?.items ?? [];
  const totalPages = data?.totalPages ?? 0;

  return (
    <>
      <section className="mb-6 flex flex-wrap items-start justify-between gap-4">
        <div>
          <h1 className="text-3xl font-bold">الإقرارات</h1>
          <p className="mt-2 text-[var(--color-text-secondary)]">
            إدارة تعريفات الإقرارات ونسخها المرتبطة بسياسات منشورة. يخضع النشر
            لقاعدة نسخة واحدة منشورة لكل تعريف.
          </p>
        </div>
        {canAuthor ? (
          <Link href="/admin/acknowledgments/new">
            <Button>+ إقرار جديد</Button>
          </Link>
        ) : null}
      </section>

      <Card>
        <CardHeader>
          <CardTitle>قائمة الإقرارات</CardTitle>
        </CardHeader>
        <CardBody>
          {/* Filter bar */}
          <div className="mb-5 flex flex-wrap items-end gap-3">
            <div className="flex-1 min-w-[220px]">
              <label
                htmlFor="ack-search"
                className="block text-xs font-medium text-[var(--color-text-tertiary)]"
              >
                بحث بالعنوان أو الإدارة
              </label>
              <input
                id="ack-search"
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
              <label
                htmlFor="ack-status"
                className="block text-xs font-medium text-[var(--color-text-tertiary)]"
              >
                الحالة
              </label>
              <select
                id="ack-status"
                value={statusFilter}
                onChange={(e) => {
                  setPage(1);
                  setStatusFilter(
                    e.target.value === ""
                      ? ""
                      : (Number(e.target.value) as AcknowledgmentStatus),
                  );
                }}
                className="mt-1 h-10 rounded-[10px] border border-[var(--color-border-strong)] bg-white px-3 text-sm"
              >
                <option value="">الكل</option>
                <option value={AcknowledgmentStatus.Draft}>مسودة</option>
                <option value={AcknowledgmentStatus.Published}>منشور</option>
                <option value={AcknowledgmentStatus.Archived}>مؤرشف</option>
              </select>
            </div>
            <div>
              <label
                htmlFor="ack-action"
                className="block text-xs font-medium text-[var(--color-text-tertiary)]"
              >
                نوع الإجراء
              </label>
              <select
                id="ack-action"
                value={actionFilter}
                onChange={(e) => {
                  setPage(1);
                  setActionFilter(
                    e.target.value === "" ? "" : (Number(e.target.value) as ActionType),
                  );
                }}
                className="mt-1 h-10 rounded-[10px] border border-[var(--color-border-strong)] bg-white px-3 text-sm"
              >
                <option value="">الكل</option>
                <option value={ActionType.SimpleAcknowledgment}>
                  {actionTypeLabel[ActionType.SimpleAcknowledgment]}
                </option>
                <option value={ActionType.AcknowledgmentWithCommitment}>
                  {actionTypeLabel[ActionType.AcknowledgmentWithCommitment]}
                </option>
                <option value={ActionType.FormBasedDisclosure}>
                  {actionTypeLabel[ActionType.FormBasedDisclosure]}
                </option>
              </select>
            </div>
            <div className="min-w-[160px]">
              <label htmlFor="ack-dept" className="block text-xs font-medium text-[var(--color-text-tertiary)]">
                الإدارة المالكة
              </label>
              <input
                id="ack-dept"
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
              <p className="text-sm text-red-700">
                تعذّر تحميل الإقرارات: {error?.title ?? "خطأ غير معروف"}
              </p>
              <Button variant="secondary" onClick={() => refetch()}>
                إعادة المحاولة
              </Button>
            </div>
          ) : items.length === 0 ? (
            <p className="text-sm text-[var(--color-text-tertiary)]">
              لا توجد تعريفات إقرار مطابقة للبحث.
            </p>
          ) : (
            <>
              <div className="overflow-x-auto">
                <table className="w-full text-right text-sm">
                  <thead className="border-b border-[var(--color-border-default)] text-xs text-[var(--color-text-tertiary)]">
                    <tr>
                      <th className="py-2 pe-4 font-medium">العنوان</th>
                      <th className="py-2 pe-4 font-medium">الإدارة المالكة</th>
                      <th className="py-2 pe-4 font-medium">نوع الإجراء</th>
                      <th className="py-2 pe-4 font-medium">الحالة</th>
                      <th className="py-2 pe-4 font-medium">النسخة الحالية</th>
                      <th className="py-2 pe-4 font-medium">عدد النسخ</th>
                      <th className="py-2 pe-4 font-medium">آخر تحديث</th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-[var(--color-border-default)]">
                    {items.map((d) => (
                      <tr key={d.id} className="hover:bg-[var(--color-surface-subtle)]">
                        <td className="py-3 pe-4">
                          <Link
                            href={`/admin/acknowledgments/${d.id}`}
                            className="font-medium text-[var(--color-text-link)] hover:underline"
                          >
                            {d.title}
                          </Link>
                        </td>
                        <td className="py-3 pe-4">{d.ownerDepartment}</td>
                        <td className="py-3 pe-4">
                          <ActionTypeBadge actionType={d.defaultActionType} />
                        </td>
                        <td className="py-3 pe-4">
                          <AcknowledgmentStatusBadge status={d.status} />
                        </td>
                        <td className="py-3 pe-4">
                          {d.currentVersionNumber ? `v${d.currentVersionNumber}` : "—"}
                        </td>
                        <td className="py-3 pe-4">{d.versionsCount}</td>
                        <td className="py-3 pe-4 text-xs text-[var(--color-text-tertiary)]">
                          {formatDate(d.updatedAtUtc ?? d.createdAtUtc)}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>

              {totalPages > 1 ? (
                <div className="mt-4 flex items-center justify-between text-sm">
                  <span className="text-[var(--color-text-tertiary)]">
                    صفحة {page} من {totalPages} — إجمالي {data?.totalCount ?? 0} إقرار
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
