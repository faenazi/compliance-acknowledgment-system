"use client";

import { useState } from "react";
import Link from "next/link";
import { Card } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { useMyHistory } from "@/lib/user-portal/hooks";
import { actionTypeLabel } from "@/lib/acknowledgments/labels";
import { formatDateTimeAr } from "@/lib/user-portal/labels";

/**
 * My History page (Sprint 6). Paginated list of all past submissions
 * by the current user.
 */
export default function MyHistoryPage() {
  const [page, setPage] = useState(1);
  const { data, isLoading } = useMyHistory(page, 20);

  return (
    <>
      <section className="mb-6">
        <h1 className="text-3xl font-bold">سجلّ الإرسالات</h1>
        <p className="mt-2 text-[var(--color-text-secondary)]">
          جميع الإقرارات والإفصاحات التي أرسلتها سابقاً.
        </p>
      </section>

      {isLoading ? (
        <Card className="p-6">
          <p className="text-sm text-[var(--color-text-tertiary)]">جاري التحميل…</p>
        </Card>
      ) : data?.items.length === 0 ? (
        <Card className="p-6">
          <p className="text-sm text-[var(--color-text-secondary)]">
            لا توجد إرسالات سابقة.
          </p>
        </Card>
      ) : (
        <>
          <div className="overflow-x-auto rounded-[14px] border border-[var(--color-border-default)] bg-white shadow-[var(--shadow-sm)]">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b border-[var(--color-border-default)] bg-[var(--color-surface-subtle)]">
                  <th className="px-4 py-3 text-start font-medium text-[var(--color-text-secondary)]">العنوان</th>
                  <th className="px-4 py-3 text-start font-medium text-[var(--color-text-secondary)]">السياسة</th>
                  <th className="px-4 py-3 text-start font-medium text-[var(--color-text-secondary)]">النوع</th>
                  <th className="px-4 py-3 text-start font-medium text-[var(--color-text-secondary)]">الإصدار</th>
                  <th className="px-4 py-3 text-start font-medium text-[var(--color-text-secondary)]">تاريخ الإرسال</th>
                  <th className="px-4 py-3 text-start font-medium text-[var(--color-text-secondary)]">الدورة</th>
                  <th className="px-4 py-3" />
                </tr>
              </thead>
              <tbody>
                {data?.items.map((item) => (
                  <tr
                    key={item.submissionId}
                    className="border-b border-[var(--color-border-soft)] last:border-0 hover:bg-[var(--color-surface-subtle)]"
                  >
                    <td className="px-4 py-3 font-medium">{item.title}</td>
                    <td className="px-4 py-3 text-[var(--color-text-secondary)]">{item.policyTitle}</td>
                    <td className="px-4 py-3 text-[var(--color-text-secondary)]">
                      {actionTypeLabel[item.actionType as keyof typeof actionTypeLabel]}
                    </td>
                    <td className="px-4 py-3 text-[var(--color-text-secondary)]">v{item.versionNumber}</td>
                    <td className="px-4 py-3 text-[var(--color-text-secondary)]">
                      <div className="flex items-center gap-2">
                        <span>{formatDateTimeAr(item.submittedAtUtc)}</span>
                        {item.isLateSubmission && (
                          <Badge status="overdue">متأخر</Badge>
                        )}
                      </div>
                    </td>
                    <td className="px-4 py-3 text-[var(--color-text-secondary)]">{item.cycleReference}</td>
                    <td className="px-4 py-3">
                      <Link href={`/history/${item.submissionId}`}>
                        <Button variant="ghost" size="sm">عرض</Button>
                      </Link>
                    </td>
                  </tr>
                ))}
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
