"use client";

import Link from "next/link";
import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";
import { useAdminRequirements } from "@/lib/admin/hooks";
import { RecurrenceModel } from "@/lib/acknowledgments/types";
import { UserActionRequirementStatus } from "@/lib/requirements/types";
import { actionTypeLabel } from "@/lib/acknowledgments/labels";
import {
  requirementStatusLabel,
  requirementStatusBadge,
  recurrenceModelLabel,
  formatDate,
  formatDateTime,
} from "@/lib/admin/labels";

const PAGE_SIZE = 20;

/**
 * Admin user action monitoring page (Sprint 7).
 * Provides a filterable, paginated table of all user action requirements
 * with status, user, department, action, and due date visibility.
 * Per admin-portal-pages.md §16.
 */
export default function AdminMonitoringPage() {
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState<UserActionRequirementStatus | "">("");
  const [deptFilter, setDeptFilter] = useState("");
  const [recurrenceFilter, setRecurrenceFilter] = useState<RecurrenceModel | "">("");
  const [dueDateFrom, setDueDateFrom] = useState("");
  const [dueDateTo, setDueDateTo] = useState("");

  const { data, isLoading, isError, error, refetch } = useAdminRequirements({
    page,
    pageSize: PAGE_SIZE,
    search: search || undefined,
    status: statusFilter === "" ? undefined : statusFilter,
    department: deptFilter || undefined,
    recurrenceModel: recurrenceFilter === "" ? undefined : recurrenceFilter,
    dueDateFrom: dueDateFrom || undefined,
    dueDateTo: dueDateTo || undefined,
    currentOnly: true,
  });

  const items = data?.items ?? [];
  const totalPages = data?.totalPages ?? 0;

  return (
    <>
      <section className="mb-6">
        <h1 className="text-3xl font-bold">متابعة إجراءات المستخدمين</h1>
        <p className="mt-2 text-[var(--color-text-secondary)]">
          مراقبة حالة الإجراءات المطلوبة من المستخدمين وتتبّع التأخيرات والإنجازات.
        </p>
      </section>

      <Card>
        <CardHeader>
          <CardTitle>الإجراءات المطلوبة</CardTitle>
        </CardHeader>
        <CardBody>
          {/* Filter bar */}
          <div className="mb-5 flex flex-wrap items-end gap-3">
            <div className="flex-1 min-w-[200px]">
              <label htmlFor="mon-search" className="block text-xs font-medium text-[var(--color-text-tertiary)]">
                بحث (مستخدم، إجراء، سياسة)
              </label>
              <input
                id="mon-search"
                type="search"
                value={search}
                onChange={(e) => { setPage(1); setSearch(e.target.value); }}
                className="mt-1 block h-10 w-full rounded-[10px] border border-[var(--color-border-strong)] bg-white px-3 text-sm outline-none focus:border-[var(--color-brand-primary)]"
              />
            </div>
            <div>
              <label htmlFor="mon-status" className="block text-xs font-medium text-[var(--color-text-tertiary)]">
                الحالة
              </label>
              <select
                id="mon-status"
                value={statusFilter}
                onChange={(e) => { setPage(1); setStatusFilter(e.target.value === "" ? "" : Number(e.target.value) as UserActionRequirementStatus); }}
                className="mt-1 h-10 rounded-[10px] border border-[var(--color-border-strong)] bg-white px-3 text-sm"
              >
                <option value="">الكل</option>
                <option value={UserActionRequirementStatus.Pending}>{requirementStatusLabel[UserActionRequirementStatus.Pending]}</option>
                <option value={UserActionRequirementStatus.Overdue}>{requirementStatusLabel[UserActionRequirementStatus.Overdue]}</option>
                <option value={UserActionRequirementStatus.Completed}>{requirementStatusLabel[UserActionRequirementStatus.Completed]}</option>
                <option value={UserActionRequirementStatus.Cancelled}>{requirementStatusLabel[UserActionRequirementStatus.Cancelled]}</option>
              </select>
            </div>
            <div className="min-w-[140px]">
              <label htmlFor="mon-dept" className="block text-xs font-medium text-[var(--color-text-tertiary)]">
                الإدارة
              </label>
              <input
                id="mon-dept"
                type="text"
                value={deptFilter}
                placeholder="الكل"
                onChange={(e) => { setPage(1); setDeptFilter(e.target.value); }}
                className="mt-1 block h-10 w-full rounded-[10px] border border-[var(--color-border-strong)] bg-white px-3 text-sm outline-none focus:border-[var(--color-brand-primary)]"
              />
            </div>
            <div>
              <label htmlFor="mon-recurrence" className="block text-xs font-medium text-[var(--color-text-tertiary)]">
                نموذج التكرار
              </label>
              <select
                id="mon-recurrence"
                value={recurrenceFilter}
                onChange={(e) => { setPage(1); setRecurrenceFilter(e.target.value === "" ? "" : Number(e.target.value) as RecurrenceModel); }}
                className="mt-1 h-10 rounded-[10px] border border-[var(--color-border-strong)] bg-white px-3 text-sm"
              >
                <option value="">الكل</option>
                <option value={RecurrenceModel.OnboardingOnly}>{recurrenceModelLabel[RecurrenceModel.OnboardingOnly]}</option>
                <option value={RecurrenceModel.Annual}>{recurrenceModelLabel[RecurrenceModel.Annual]}</option>
                <option value={RecurrenceModel.OnboardingAndAnnual}>{recurrenceModelLabel[RecurrenceModel.OnboardingAndAnnual]}</option>
                <option value={RecurrenceModel.OnChange}>{recurrenceModelLabel[RecurrenceModel.OnChange]}</option>
                <option value={RecurrenceModel.EventDriven}>{recurrenceModelLabel[RecurrenceModel.EventDriven]}</option>
              </select>
            </div>
            <div>
              <label htmlFor="mon-due-from" className="block text-xs font-medium text-[var(--color-text-tertiary)]">
                تاريخ الاستحقاق من
              </label>
              <input
                id="mon-due-from"
                type="date"
                value={dueDateFrom}
                onChange={(e) => { setPage(1); setDueDateFrom(e.target.value); }}
                className="mt-1 h-10 rounded-[10px] border border-[var(--color-border-strong)] bg-white px-3 text-sm"
              />
            </div>
            <div>
              <label htmlFor="mon-due-to" className="block text-xs font-medium text-[var(--color-text-tertiary)]">
                تاريخ الاستحقاق إلى
              </label>
              <input
                id="mon-due-to"
                type="date"
                value={dueDateTo}
                onChange={(e) => { setPage(1); setDueDateTo(e.target.value); }}
                className="mt-1 h-10 rounded-[10px] border border-[var(--color-border-strong)] bg-white px-3 text-sm"
              />
            </div>
          </div>

          {isLoading ? (
            <p className="text-sm text-[var(--color-text-tertiary)]">جاري التحميل…</p>
          ) : isError ? (
            <div className="space-y-3">
              <p className="text-sm text-red-700">
                تعذّر تحميل الإجراءات: {error?.title ?? "خطأ غير معروف"}
              </p>
              <Button variant="secondary" onClick={() => refetch()}>
                إعادة المحاولة
              </Button>
            </div>
          ) : items.length === 0 ? (
            <p className="text-sm text-[var(--color-text-tertiary)]">
              لا توجد إجراءات مطابقة للفلتر المحدد.
            </p>
          ) : (
            <>
              <div className="overflow-x-auto">
                <table className="w-full text-right text-sm">
                  <thead className="border-b border-[var(--color-border-default)] text-xs text-[var(--color-text-tertiary)]">
                    <tr>
                      <th className="py-2 pe-4 font-medium">المستخدم</th>
                      <th className="py-2 pe-4 font-medium">الإدارة</th>
                      <th className="py-2 pe-4 font-medium">الإجراء</th>
                      <th className="py-2 pe-4 font-medium">النسخة</th>
                      <th className="py-2 pe-4 font-medium">الحالة</th>
                      <th className="py-2 pe-4 font-medium">تاريخ التكليف</th>
                      <th className="py-2 pe-4 font-medium">تاريخ الاستحقاق</th>
                      <th className="py-2 pe-4 font-medium">تاريخ الإنجاز</th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-[var(--color-border-default)]">
                    {items.map((r) => (
                      <tr key={r.requirementId} className="hover:bg-[var(--color-surface-subtle)]">
                        <td className="py-3 pe-4">
                          <Link
                            href={`/admin/monitoring/${r.requirementId}`}
                            className="font-medium text-[var(--color-text-link)] hover:underline"
                          >
                            {r.userDisplayName}
                          </Link>
                        </td>
                        <td className="py-3 pe-4 text-xs">{r.userDepartment}</td>
                        <td className="py-3 pe-4">
                          <div>{r.actionTitle}</div>
                          <div className="text-xs text-[var(--color-text-tertiary)]">
                            {actionTypeLabel[r.actionType]}
                          </div>
                        </td>
                        <td className="py-3 pe-4 text-xs">v{r.versionNumber}</td>
                        <td className="py-3 pe-4">
                          <Badge status={requirementStatusBadge[r.status]}>
                            {requirementStatusLabel[r.status]}
                          </Badge>
                        </td>
                        <td className="py-3 pe-4 text-xs">{formatDate(r.assignedAtUtc)}</td>
                        <td className="py-3 pe-4 text-xs">{formatDate(r.dueDate)}</td>
                        <td className="py-3 pe-4 text-xs">{formatDateTime(r.completedAtUtc)}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>

              {totalPages > 1 ? (
                <div className="mt-4 flex items-center justify-between text-sm">
                  <span className="text-[var(--color-text-tertiary)]">
                    صفحة {page} من {totalPages} — إجمالي {data?.totalCount ?? 0} إجراء
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
