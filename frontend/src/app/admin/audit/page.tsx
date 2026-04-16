"use client";

import { useState } from "react";
import { Card, CardBody } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { useAuditLogs } from "@/lib/audit/hooks";
import { entityTypeLabel, actionTypeLabel } from "@/lib/audit/labels";
import { formatDateTime } from "@/lib/admin/labels";
import { Search, Download, ChevronRight, ChevronLeft, Shield } from "lucide-react";

/**
 * Audit Log Explorer page (Sprint 8, admin-portal-pages §19).
 * Searchable, filterable audit log table for governance and audit review.
 */
export default function AuditLogExplorerPage() {
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState("");
  const [entityType, setEntityType] = useState("");
  const [actionType, setActionType] = useState("");
  const [fromDate, setFromDate] = useState("");
  const [toDate, setToDate] = useState("");
  const pageSize = 20;

  const { data, isLoading, isError, error, refetch } = useAuditLogs({
    page,
    pageSize,
    search: search || undefined,
    entityType: entityType || undefined,
    actionType: actionType || undefined,
    fromDate: fromDate || undefined,
    toDate: toDate || undefined,
  });

  const apiBase = process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:5100";
  const exportParams = new URLSearchParams();
  if (entityType) exportParams.set("entityType", entityType);
  if (actionType) exportParams.set("actionType", actionType);
  if (fromDate) exportParams.set("fromDate", fromDate);
  if (toDate) exportParams.set("toDate", toDate);
  if (search) exportParams.set("search", search);
  const exportUrl = `${apiBase}/api/admin/audit/export${exportParams.toString() ? `?${exportParams.toString()}` : ""}`;

  const knownEntityTypes = [
    "Policy", "PolicyVersion", "AcknowledgmentDefinition", "AcknowledgmentVersion",
    "UserSubmission", "UserActionRequirement", "FormDefinition", "AudienceDefinition", "User",
  ];

  return (
    <>
      <section className="mb-6 flex items-start justify-between gap-4">
        <div>
          <h1 className="text-3xl font-bold flex items-center gap-3">
            <Shield size={28} />
            سجل المراجعة
          </h1>
          <p className="mt-2 text-[var(--color-text-secondary)]">
            سجل الأحداث الإدارية والتشغيلية لأغراض الحوكمة والمراجعة.
          </p>
        </div>
        <a href={exportUrl} target="_blank" rel="noopener noreferrer">
          <Button variant="secondary" size="sm">
            <Download size={14} className="ml-1" />
            تصدير CSV
          </Button>
        </a>
      </section>

      {/* Filter bar */}
      <div className="mb-6 flex flex-wrap gap-3">
        <div className="relative">
          <Search size={16} className="absolute top-1/2 right-3 -translate-y-1/2 text-[var(--color-text-disabled)]" />
          <input
            type="text"
            placeholder="بحث بالاسم أو الوصف…"
            value={search}
            onChange={(e) => { setSearch(e.target.value); setPage(1); }}
            className="h-[44px] rounded-[10px] border border-[var(--color-border-default)] bg-white pr-9 pl-4 text-sm focus:border-[var(--color-brand-primary)] focus:outline-none"
          />
        </div>
        <select
          value={entityType}
          onChange={(e) => { setEntityType(e.target.value); setPage(1); }}
          className="h-[44px] rounded-[10px] border border-[var(--color-border-default)] bg-white px-4 text-sm focus:border-[var(--color-brand-primary)] focus:outline-none"
        >
          <option value="">كل الكيانات</option>
          {knownEntityTypes.map((t) => (
            <option key={t} value={t}>{entityTypeLabel[t] ?? t}</option>
          ))}
        </select>
        <input
          type="text"
          placeholder="نوع الإجراء"
          value={actionType}
          onChange={(e) => { setActionType(e.target.value); setPage(1); }}
          className="h-[44px] rounded-[10px] border border-[var(--color-border-default)] bg-white px-4 text-sm focus:border-[var(--color-brand-primary)] focus:outline-none"
        />
        <input
          type="date"
          value={fromDate}
          onChange={(e) => { setFromDate(e.target.value); setPage(1); }}
          className="h-[44px] rounded-[10px] border border-[var(--color-border-default)] bg-white px-4 text-sm focus:border-[var(--color-brand-primary)] focus:outline-none"
          title="من تاريخ"
        />
        <input
          type="date"
          value={toDate}
          onChange={(e) => { setToDate(e.target.value); setPage(1); }}
          className="h-[44px] rounded-[10px] border border-[var(--color-border-default)] bg-white px-4 text-sm focus:border-[var(--color-brand-primary)] focus:outline-none"
          title="إلى تاريخ"
        />
      </div>

      {isLoading ? (
        <p className="text-sm text-[var(--color-text-tertiary)]">جاري تحميل سجل المراجعة…</p>
      ) : isError ? (
        <div className="space-y-3">
          <p className="text-sm text-red-700">
            تعذّر تحميل السجل: {error?.title ?? "خطأ غير معروف"}
          </p>
          <Button variant="secondary" onClick={() => refetch()}>إعادة المحاولة</Button>
        </div>
      ) : data ? (
        <Card>
          <CardBody>
            {data.items.length === 0 ? (
              <p className="py-8 text-center text-sm text-[var(--color-text-tertiary)]">
                لا توجد سجلات مطابقة.
              </p>
            ) : (
              <>
                <div className="overflow-x-auto">
                  <table className="w-full text-sm">
                    <thead>
                      <tr className="border-b border-[var(--color-border-default)] bg-[var(--color-surface-subtle)]">
                        <th className="px-4 py-3 text-right font-medium">التاريخ والوقت</th>
                        <th className="px-4 py-3 text-right font-medium">المستخدم</th>
                        <th className="px-4 py-3 text-right font-medium">الإجراء</th>
                        <th className="px-4 py-3 text-right font-medium">الكيان</th>
                        <th className="px-4 py-3 text-right font-medium">الوصف</th>
                      </tr>
                    </thead>
                    <tbody>
                      {data.items.map((log) => (
                        <tr key={log.id} className="border-b border-[var(--color-border-soft)] hover:bg-[var(--color-surface-subtle)]">
                          <td className="px-4 py-3 text-xs whitespace-nowrap">
                            {formatDateTime(log.actionTimestampUtc)}
                          </td>
                          <td className="px-4 py-3">
                            {log.actorUsername ?? (
                              <span className="text-[var(--color-text-tertiary)]">النظام</span>
                            )}
                          </td>
                          <td className="px-4 py-3">
                            <Badge variant="published">
                              {actionTypeLabel[log.actionType] ?? log.actionType}
                            </Badge>
                          </td>
                          <td className="px-4 py-3">
                            <span className="text-xs">
                              {entityTypeLabel[log.entityType] ?? log.entityType}
                            </span>
                            {log.entityId ? (
                              <span className="mr-1 text-xs text-[var(--color-text-tertiary)]">
                                ({log.entityId.slice(0, 8)}…)
                              </span>
                            ) : null}
                          </td>
                          <td className="px-4 py-3 max-w-[300px] truncate text-[var(--color-text-secondary)]">
                            {log.description ?? "—"}
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>

                {/* Pagination */}
                <div className="mt-4 flex items-center justify-between border-t border-[var(--color-border-soft)] pt-4">
                  <p className="text-xs text-[var(--color-text-tertiary)]">
                    إجمالي {data.totalCount} سجل — صفحة {data.page} من {data.totalPages}
                  </p>
                  <div className="flex gap-2">
                    <Button
                      variant="ghost"
                      size="sm"
                      disabled={!data.hasPrevious}
                      onClick={() => setPage((p) => Math.max(1, p - 1))}
                    >
                      <ChevronRight size={14} />
                      السابق
                    </Button>
                    <Button
                      variant="ghost"
                      size="sm"
                      disabled={!data.hasNext}
                      onClick={() => setPage((p) => p + 1)}
                    >
                      التالي
                      <ChevronLeft size={14} />
                    </Button>
                  </div>
                </div>
              </>
            )}
          </CardBody>
        </Card>
      ) : null}
    </>
  );
}
