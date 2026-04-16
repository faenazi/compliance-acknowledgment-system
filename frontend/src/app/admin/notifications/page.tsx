"use client";

import { useState } from "react";
import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import {
  useNotifications,
  useSendAssignmentNotifications,
  useSendReminderNotifications,
  useSendOverdueNotifications,
} from "@/lib/audit/hooks";
import { notificationTypeLabel, notificationStatusLabel } from "@/lib/compliance/labels";
import { formatDateTime } from "@/lib/admin/labels";
import { Bell, Send, ChevronRight, ChevronLeft, Search } from "lucide-react";

/**
 * Notification operations page (Sprint 8).
 * Allows admins to trigger notifications and view the notification log.
 */
export default function NotificationsPage() {
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState("");
  const [typeFilter, setTypeFilter] = useState("");
  const [statusFilter, setStatusFilter] = useState("");
  const pageSize = 20;

  const { data, isLoading, refetch } = useNotifications({
    page,
    pageSize,
    search: search || undefined,
    type: typeFilter ? Number(typeFilter) : undefined,
    status: statusFilter ? Number(statusFilter) : undefined,
  });

  const sendAssignments = useSendAssignmentNotifications();
  const sendReminders = useSendReminderNotifications();
  const sendOverdue = useSendOverdueNotifications();
  const [lastResult, setLastResult] = useState<string | null>(null);

  const handleSend = async (
    action: () => Promise<{ sent: number; failed: number; skipped: number }>,
    label: string,
  ) => {
    try {
      const result = await action();
      setLastResult(`${label}: أُرسل ${result.sent}، فشل ${result.failed}، تُخطّي ${result.skipped}`);
      refetch();
    } catch {
      setLastResult(`${label}: حدث خطأ أثناء الإرسال.`);
    }
  };

  const statusBadgeVariant = (status: number) => {
    switch (status) {
      case 1: return "completed" as const;
      case 2: return "overdue" as const;
      case 3: return "archived" as const;
      default: return "pending" as const;
    }
  };

  return (
    <>
      <section className="mb-6">
        <h1 className="text-3xl font-bold flex items-center gap-3">
          <Bell size={28} />
          الإشعارات
        </h1>
        <p className="mt-2 text-[var(--color-text-secondary)]">
          إدارة إرسال الإشعارات ومراجعة سجل التسليم.
        </p>
      </section>

      {/* Send Actions */}
      <Card className="mb-6">
        <CardHeader>
          <CardTitle>إرسال الإشعارات</CardTitle>
        </CardHeader>
        <CardBody>
          <div className="flex flex-wrap gap-3">
            <Button
              variant="primary"
              size="sm"
              disabled={sendAssignments.isPending}
              onClick={() =>
                handleSend(
                  () => sendAssignments.mutateAsync(undefined),
                  "إشعارات التعيين",
                )
              }
            >
              <Send size={14} className="ml-1" />
              إرسال إشعارات التعيين
            </Button>
            <Button
              variant="secondary"
              size="sm"
              disabled={sendReminders.isPending}
              onClick={() =>
                handleSend(
                  () => sendReminders.mutateAsync(undefined),
                  "إشعارات التذكير",
                )
              }
            >
              <Send size={14} className="ml-1" />
              إرسال التذكيرات
            </Button>
            <Button
              variant="secondary"
              size="sm"
              disabled={sendOverdue.isPending}
              onClick={() =>
                handleSend(
                  () => sendOverdue.mutateAsync(),
                  "إشعارات التأخّر",
                )
              }
            >
              <Send size={14} className="ml-1" />
              إرسال إشعارات التأخّر
            </Button>
          </div>
          {lastResult ? (
            <p className="mt-3 text-sm text-[var(--color-text-secondary)]">{lastResult}</p>
          ) : null}
        </CardBody>
      </Card>

      {/* Notification Log Filters */}
      <div className="mb-4 flex flex-wrap gap-3">
        <div className="relative">
          <Search size={16} className="absolute top-1/2 right-3 -translate-y-1/2 text-[var(--color-text-disabled)]" />
          <input
            type="text"
            placeholder="بحث بالبريد أو الموضوع…"
            value={search}
            onChange={(e) => { setSearch(e.target.value); setPage(1); }}
            className="h-[44px] rounded-[10px] border border-[var(--color-border-default)] bg-white pr-9 pl-4 text-sm focus:border-[var(--color-brand-primary)] focus:outline-none"
          />
        </div>
        <select
          value={typeFilter}
          onChange={(e) => { setTypeFilter(e.target.value); setPage(1); }}
          className="h-[44px] rounded-[10px] border border-[var(--color-border-default)] bg-white px-4 text-sm focus:border-[var(--color-brand-primary)] focus:outline-none"
        >
          <option value="">كل الأنواع</option>
          <option value="0">تعيين</option>
          <option value="1">تذكير</option>
          <option value="2">تأخّر</option>
        </select>
        <select
          value={statusFilter}
          onChange={(e) => { setStatusFilter(e.target.value); setPage(1); }}
          className="h-[44px] rounded-[10px] border border-[var(--color-border-default)] bg-white px-4 text-sm focus:border-[var(--color-brand-primary)] focus:outline-none"
        >
          <option value="">كل الحالات</option>
          <option value="0">في الانتظار</option>
          <option value="1">تم الإرسال</option>
          <option value="2">فشل</option>
        </select>
      </div>

      {/* Notification Log Table */}
      {isLoading ? (
        <p className="text-sm text-[var(--color-text-tertiary)]">جاري تحميل سجل الإشعارات…</p>
      ) : data ? (
        <Card>
          <CardBody>
            {data.items.length === 0 ? (
              <p className="py-8 text-center text-sm text-[var(--color-text-tertiary)]">
                لا توجد إشعارات مسجلة.
              </p>
            ) : (
              <>
                <div className="overflow-x-auto">
                  <table className="w-full text-sm">
                    <thead>
                      <tr className="border-b border-[var(--color-border-default)] bg-[var(--color-surface-subtle)]">
                        <th className="px-4 py-3 text-right font-medium">التاريخ</th>
                        <th className="px-4 py-3 text-right font-medium">المستلم</th>
                        <th className="px-4 py-3 text-right font-medium">النوع</th>
                        <th className="px-4 py-3 text-right font-medium">الموضوع</th>
                        <th className="px-4 py-3 text-right font-medium">الحالة</th>
                        <th className="px-4 py-3 text-right font-medium">المحاولات</th>
                      </tr>
                    </thead>
                    <tbody>
                      {data.items.map((n) => (
                        <tr key={n.id} className="border-b border-[var(--color-border-soft)] hover:bg-[var(--color-surface-subtle)]">
                          <td className="px-4 py-3 text-xs whitespace-nowrap">
                            {formatDateTime(n.createdAtUtc)}
                          </td>
                          <td className="px-4 py-3">
                            <div>{n.recipientName ?? "—"}</div>
                            <div className="text-xs text-[var(--color-text-tertiary)]">{n.recipientEmail}</div>
                          </td>
                          <td className="px-4 py-3">
                            <Badge variant="pending">
                              {notificationTypeLabel[n.notificationType] ?? String(n.notificationType)}
                            </Badge>
                          </td>
                          <td className="px-4 py-3 max-w-[250px] truncate">{n.subject}</td>
                          <td className="px-4 py-3">
                            <Badge variant={statusBadgeVariant(n.status)}>
                              {notificationStatusLabel[n.status] ?? String(n.status)}
                            </Badge>
                          </td>
                          <td className="px-4 py-3 text-center">
                            {n.attemptCount}
                            {n.lastFailureReason ? (
                              <div className="mt-1 text-xs text-[#9A3412] max-w-[200px] truncate" title={n.lastFailureReason}>
                                {n.lastFailureReason}
                              </div>
                            ) : null}
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
                    <Button variant="ghost" size="sm" disabled={!data.hasPrevious} onClick={() => setPage((p) => Math.max(1, p - 1))}>
                      <ChevronRight size={14} /> السابق
                    </Button>
                    <Button variant="ghost" size="sm" disabled={!data.hasNext} onClick={() => setPage((p) => p + 1)}>
                      التالي <ChevronLeft size={14} />
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
