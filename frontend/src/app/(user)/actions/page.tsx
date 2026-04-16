"use client";

import { useState } from "react";
import Link from "next/link";
import { Card } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { RequirementStatusBadge } from "@/components/user-portal/RequirementStatusBadge";
import { useMyActions } from "@/lib/user-portal/hooks";
import { actionTypeLabel } from "@/lib/acknowledgments/labels";
import { recurrenceModelLabel } from "@/lib/acknowledgments/recurrenceLabels";
import { formatDateAr } from "@/lib/user-portal/labels";
import { UserActionRequirementStatus } from "@/lib/user-portal/types";

const STATUS_FILTERS = [
  { value: undefined, label: "الكل" },
  { value: UserActionRequirementStatus.Pending, label: "معلّق" },
  { value: UserActionRequirementStatus.Overdue, label: "متأخر" },
  { value: UserActionRequirementStatus.Completed, label: "مكتمل" },
] as const;

/**
 * My Required Actions list page (Sprint 6). Searchable, filterable list
 * of all current action requirements assigned to the user.
 */
export default function MyActionsPage() {
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState<UserActionRequirementStatus | undefined>(undefined);

  const { data, isLoading } = useMyActions({
    page,
    pageSize: 20,
    status: statusFilter,
    search: search || undefined,
  });

  return (
    <>
      <section className="mb-6">
        <h1 className="text-3xl font-bold">الإجراءات المطلوبة</h1>
        <p className="mt-2 text-[var(--color-text-secondary)]">
          جميع الإقرارات والإفصاحات المطلوبة منك.
        </p>
      </section>

      {/* Filters */}
      <div className="mb-6 flex flex-wrap items-center gap-3">
        <input
          type="text"
          placeholder="بحث بالعنوان أو السياسة…"
          value={search}
          onChange={(e) => {
            setSearch(e.target.value);
            setPage(1);
          }}
          className="h-11 w-full max-w-xs rounded-[10px] border border-[var(--color-border-default)] bg-white px-3 text-sm"
        />
        <div className="flex gap-1">
          {STATUS_FILTERS.map((f) => (
            <button
              key={f.label}
              onClick={() => {
                setStatusFilter(f.value);
                setPage(1);
              }}
              className={`rounded-full px-3 py-1.5 text-xs font-medium transition-colors ${
                statusFilter === f.value
                  ? "bg-[var(--color-brand-primary)] text-white"
                  : "bg-[var(--color-surface-subtle)] text-[var(--color-text-secondary)] hover:bg-[var(--color-surface-soft)]"
              }`}
            >
              {f.label}
            </button>
          ))}
        </div>
      </div>

      {/* Results */}
      {isLoading ? (
        <Card className="p-6">
          <p className="text-sm text-[var(--color-text-tertiary)]">جاري التحميل…</p>
        </Card>
      ) : data?.items.length === 0 ? (
        <Card className="p-6">
          <p className="text-sm text-[var(--color-text-secondary)]">
            لا توجد نتائج.
          </p>
        </Card>
      ) : (
        <>
          {/* Table */}
          <div className="overflow-x-auto rounded-[14px] border border-[var(--color-border-default)] bg-white shadow-[var(--shadow-sm)]">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b border-[var(--color-border-default)] bg-[var(--color-surface-subtle)]">
                  <th className="px-4 py-3 text-start font-medium text-[var(--color-text-secondary)]">العنوان</th>
                  <th className="px-4 py-3 text-start font-medium text-[var(--color-text-secondary)]">السياسة</th>
                  <th className="px-4 py-3 text-start font-medium text-[var(--color-text-secondary)]">النوع</th>
                  <th className="px-4 py-3 text-start font-medium text-[var(--color-text-secondary)]">الاستحقاق</th>
                  <th className="px-4 py-3 text-start font-medium text-[var(--color-text-secondary)]">الحالة</th>
                  <th className="px-4 py-3 text-start font-medium text-[var(--color-text-secondary)]">التكرار</th>
                  <th className="px-4 py-3" />
                </tr>
              </thead>
              <tbody>
                {data?.items.map((action) => {
                  const isOverdue = action.status === UserActionRequirementStatus.Overdue;
                  return (
                    <tr
                      key={action.requirementId}
                      className={`border-b border-[var(--color-border-soft)] last:border-0 hover:bg-[var(--color-surface-subtle)] ${
                        isOverdue ? "bg-[var(--color-status-overdue-bg)]" : ""
                      }`}
                    >
                      <td className="px-4 py-3 font-medium">{action.title}</td>
                      <td className="px-4 py-3 text-[var(--color-text-secondary)]">{action.policyTitle}</td>
                      <td className="px-4 py-3 text-[var(--color-text-secondary)]">
                        {actionTypeLabel[action.actionType as keyof typeof actionTypeLabel]}
                      </td>
                      <td className={`px-4 py-3 ${isOverdue ? "font-medium text-[var(--color-status-overdue-text)]" : "text-[var(--color-text-secondary)]"}`}>
                        {formatDateAr(action.dueDate)}
                      </td>
                      <td className="px-4 py-3">
                        <RequirementStatusBadge status={action.status} />
                      </td>
                      <td className="px-4 py-3 text-[var(--color-text-secondary)]">
                        {recurrenceModelLabel(action.recurrenceModel)}
                      </td>
                      <td className="px-4 py-3">
                        <Link href={`/actions/${action.requirementId}`}>
                          <Button variant="ghost" size="sm">عرض</Button>
                        </Link>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>

          {/* Pagination */}
          {data && data.totalPages > 1 && (
            <div className="mt-4 flex items-center justify-center gap-2">
              <Button
                variant="secondary"
                size="sm"
                disabled={!data.hasPrevious}
                onClick={() => setPage((p) => p - 1)}
              >
                السابق
              </Button>
              <span className="text-sm text-[var(--color-text-secondary)]">
                صفحة {data.page} من {data.totalPages}
              </span>
              <Button
                variant="secondary"
                size="sm"
                disabled={!data.hasNext}
                onClick={() => setPage((p) => p + 1)}
              >
                التالي
              </Button>
            </div>
          )}
        </>
      )}
    </>
  );
}
