"use client";

import { useState } from "react";
import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { useNonCompliantUsers } from "@/lib/compliance/hooks";
import { requirementStatusLabel, requirementStatusBadge, formatDate, formatDateTime } from "@/lib/admin/labels";
import type { UserActionRequirementStatus } from "@/lib/requirements/types";
import { Download, ChevronRight, ChevronLeft, Search } from "lucide-react";
import Link from "next/link";

/**
 * Compliance Reports page (Sprint 8, admin-portal-pages §18).
 * Shows non-compliant user details with filters and export.
 */
export default function ComplianceReportsPage() {
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState("");
  const [department, setDepartment] = useState("");
  const [statusFilter, setStatusFilter] = useState<string>("");
  const pageSize = 20;

  const { data, isLoading, isError, error, refetch } = useNonCompliantUsers({
    page,
    pageSize,
    search: search || undefined,
    department: department || undefined,
    status: statusFilter ? (Number(statusFilter) as UserActionRequirementStatus) : undefined,
  });

  const apiBase = process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:5100";

  return (
    <>
      <section className="mb-6 flex items-start justify-between gap-4">
        <div>
          <h1 className="text-3xl font-bold">تقارير الامتثال</h1>
          <p className="mt-2 text-[var(--color-text-secondary)]">
            تقرير المستخدمين غير الممتثلين — معلّق ومتأخر.
          </p>
        </div>
        <div className="flex gap-2">
          <Link href="/admin/compliance">
            <Button variant="ghost" size="sm">العودة للوحة الامتثال</Button>
          </Link>
          <a
            href={`${apiBase}/api/admin/compliance/export/non-compliant${department ? `?department=${encodeURIComponent(department)}` : ""}`}
            target="_blank"
            rel="noopener noreferrer"
          >
            <Button variant="secondary" size="sm">
              <Download size={14} className="ml-1" />
              تصدير CSV
            </Button>
          </a>
        </div>
      </section>

      {/* Filters */}
      <div className="mb-6 flex flex-wrap gap-3">
        <div className="relative">
          <Search size={16} className="absolute top-1/2 right-3 -translate-y-1/2 text-[var(--color-text-disabled)]" />
          <input
            type="text"
            placeholder="بحث بالاسم أو الإجراء..."
            value={search}
            onChange={(e) => { setSearch(e.target.value); setPage(1); }}
            className="h-[44px] rounded-[10px] border border-[var(--color-border-default)] bg-white pr-9 pl-4 text-sm focus:border-[var(--color-brand-primary)] focus:outline-none"
          />
        </div>
        <input
          type="text"
          placeholder="القسم"
          value={department}
          onChange={(e) => { setDepartment(e.target.value); setPage(1); }}
          className="h-[44px] rounded-[10px] border border-[var(--color-border-default)] bg-white px-4 text-sm focus:border-[var(--color-brand-primary)] focus:outline-none"
        />
        <select
          value={statusFilter}
          onChange={(e) => { setStatusFilter(e.target.value); setPage(1); }}
          className="h-[44px] rounded-[10px] border border-[var(--color-border-default)] bg-white px-4 text-sm focus:border-[var(--color-brand-primary)] focus:outline-none"
        >
          <option value="">كل الحالات</option>
          <option value="0">معلّق</option>
          <option value="2">متأخر</option>
        </select>
      </div>

      {isLoading ? (
        <p className="text-sm text-[var(--color-text-tertiary)]">جاري تحميل التقرير…</p>
      ) : isError ? (
        <div className="space-y-3">
          <p className="text-sm text-red-700">
            تعذّر تحميل التقرير: {error?.title ?? "خطأ غير معروف"}
          </p>
          <Button variant="secondary" onClick={() => refetch()}>إعادة المحاولة</Button>
        </div>
      ) : data ? (
        <Card>
          <CardBody>
            {data.items.length === 0 ? (
              <p className="py-8 text-center text-sm text-[var(--color-text-tertiary)]">
                لا يوجد مستخدمون غير ممتثلين بناءً على المعايير المحددة.
              </p>
            ) : (
              <>
                <div className="overflow-x-auto">
                  <table className="w-full text-sm">
                    <thead>
                      <tr className="border-b border-[var(--color-border-default)] bg-[var(--color-surface-subtle)]">
                        <th className="px-4 py-3 text-right font-medium">الاسم</th>
                        <th className="px-4 py-3 text-right font-medium">القسم</th>
                        <th className="px-4 py-3 text-right font-medium">الإجراء</th>
                        <th className="px-4 py-3 text-right font-medium">الحالة</th>
                        <th className="px-4 py-3 text-right font-medium">تاريخ الاستحقاق</th>
                        <th className="px-4 py-3 text-right font-medium">تاريخ التعيين</th>
                        <th className="px-4 py-3 text-right font-medium">الدورة</th>
                      </tr>
                    </thead>
                    <tbody>
                      {data.items.map((item) => (
                        <tr key={item.requirementId} className="border-b border-[var(--color-border-soft)] hover:bg-[var(--color-surface-subtle)]">
                          <td className="px-4 py-3 font-medium">{item.displayName}</td>
                          <td className="px-4 py-3 text-[var(--color-text-secondary)]">{item.department}</td>
                          <td className="px-4 py-3">{item.actionTitle}</td>
                          <td className="px-4 py-3">
                            <Badge variant={requirementStatusBadge[item.status]}>
                              {requirementStatusLabel[item.status]}
                            </Badge>
                          </td>
                          <td className="px-4 py-3">{formatDate(item.dueDate)}</td>
                          <td className="px-4 py-3">{formatDate(item.assignedAtUtc)}</td>
                          <td className="px-4 py-3 text-xs text-[var(--color-text-tertiary)]">{item.cycleReference}</td>
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
