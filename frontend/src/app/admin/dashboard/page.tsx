"use client";

import Link from "next/link";
import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { KpiCard } from "@/components/admin/KpiCard";
import { useAdminDashboard } from "@/lib/admin/hooks";
import { formatDateTime } from "@/lib/admin/labels";
import { useSession } from "@/lib/auth/SessionProvider";
import {
  FileText,
  ClipboardCheck,
  Clock,
  AlertTriangle,
  CheckCircle,
  TrendingUp,
} from "lucide-react";

/**
 * Admin portal operational dashboard (Sprint 7).
 * Replaces the placeholder from Sprint 1 with real KPI cards,
 * recently published items, and quick navigation links.
 */
export default function AdminDashboardPage() {
  const { user } = useSession();
  const { data, isLoading, isError, error, refetch } = useAdminDashboard(5);

  return (
    <>
      <section className="mb-6">
        <h1 className="text-3xl font-bold">لوحة الإدارة</h1>
        <p className="mt-2 text-[var(--color-text-secondary)]">
          {user
            ? `مرحباً ${user.displayName} — ملخص العمليات التشغيلية للمنصة.`
            : "جاري تحميل البيانات…"}
        </p>
      </section>

      {isLoading ? (
        <p className="text-sm text-[var(--color-text-tertiary)]">جاري تحميل لوحة الإدارة…</p>
      ) : isError ? (
        <div className="space-y-3">
          <p className="text-sm text-red-700">
            تعذّر تحميل لوحة الإدارة: {error?.title ?? "خطأ غير معروف"}
          </p>
          <Button variant="secondary" onClick={() => refetch()}>
            إعادة المحاولة
          </Button>
        </div>
      ) : data ? (
        <>
          {/* KPI Cards */}
          <div className="mb-8 grid gap-4 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-6">
            <KpiCard
              label="السياسات النشطة"
              value={data.activePolicies}
              icon={<FileText size={20} />}
            />
            <KpiCard
              label="الإقرارات النشطة"
              value={data.activeAcknowledgments}
              icon={<ClipboardCheck size={20} />}
            />
            <KpiCard
              label="إجراءات معلّقة"
              value={data.pendingUserActions}
              icon={<Clock size={20} />}
              accentClass="text-[var(--color-brand-primary)]"
            />
            <KpiCard
              label="إجراءات متأخرة"
              value={data.overdueUserActions}
              icon={<AlertTriangle size={20} />}
              accentClass={data.overdueUserActions > 0 ? "text-[#9A3412]" : undefined}
            />
            <KpiCard
              label="إجراءات مكتملة"
              value={data.completedUserActions}
              icon={<CheckCircle size={20} />}
              accentClass="text-[#5C6B1E]"
            />
            <KpiCard
              label="نسبة الإنجاز"
              value={`${data.completionRate}%`}
              icon={<TrendingUp size={20} />}
              subtitle={`${data.completedUserActions} من ${data.totalUserActions}`}
            />
          </div>

          {/* Recently published + Quick links */}
          <div className="grid gap-6 lg:grid-cols-2">
            {/* Recently published policies */}
            <Card>
              <CardHeader>
                <CardTitle>آخر السياسات المنشورة</CardTitle>
              </CardHeader>
              <CardBody>
                {data.recentlyPublishedPolicies.length === 0 ? (
                  <p className="text-sm text-[var(--color-text-tertiary)]">لا توجد سياسات منشورة بعد.</p>
                ) : (
                  <div className="space-y-3">
                    {data.recentlyPublishedPolicies.map((item) => (
                      <div
                        key={item.versionId}
                        className="flex items-center justify-between gap-3 rounded-[10px] border border-[var(--color-border-default)] p-3"
                      >
                        <div className="min-w-0">
                          <Link
                            href={`/admin/policies/${item.id}`}
                            className="text-sm font-medium text-[var(--color-text-link)] hover:underline"
                          >
                            {item.title}
                          </Link>
                          <p className="text-xs text-[var(--color-text-tertiary)]">
                            v{item.versionNumber} · {item.ownerDepartment}
                          </p>
                        </div>
                        <span className="shrink-0 text-xs text-[var(--color-text-tertiary)]">
                          {formatDateTime(item.publishedAtUtc)}
                        </span>
                      </div>
                    ))}
                  </div>
                )}
              </CardBody>
            </Card>

            {/* Recently published acknowledgments */}
            <Card>
              <CardHeader>
                <CardTitle>آخر الإقرارات المنشورة</CardTitle>
              </CardHeader>
              <CardBody>
                {data.recentlyPublishedAcknowledgments.length === 0 ? (
                  <p className="text-sm text-[var(--color-text-tertiary)]">لا توجد إقرارات منشورة بعد.</p>
                ) : (
                  <div className="space-y-3">
                    {data.recentlyPublishedAcknowledgments.map((item) => (
                      <div
                        key={item.versionId}
                        className="flex items-center justify-between gap-3 rounded-[10px] border border-[var(--color-border-default)] p-3"
                      >
                        <div className="min-w-0">
                          <Link
                            href={`/admin/acknowledgments/${item.id}`}
                            className="text-sm font-medium text-[var(--color-text-link)] hover:underline"
                          >
                            {item.title}
                          </Link>
                          <p className="text-xs text-[var(--color-text-tertiary)]">
                            v{item.versionNumber} · {item.ownerDepartment}
                          </p>
                        </div>
                        <span className="shrink-0 text-xs text-[var(--color-text-tertiary)]">
                          {formatDateTime(item.publishedAtUtc)}
                        </span>
                      </div>
                    ))}
                  </div>
                )}
              </CardBody>
            </Card>
          </div>

          {/* Quick links */}
          <div className="mt-6">
            <Card>
              <CardHeader>
                <CardTitle>روابط سريعة</CardTitle>
              </CardHeader>
              <CardBody>
                <div className="flex flex-wrap gap-3">
                  <Link href="/admin/policies">
                    <Button variant="secondary" size="sm">إدارة السياسات</Button>
                  </Link>
                  <Link href="/admin/acknowledgments">
                    <Button variant="secondary" size="sm">إدارة الإقرارات</Button>
                  </Link>
                  <Link href="/admin/monitoring">
                    <Button variant="secondary" size="sm">متابعة الإجراءات</Button>
                  </Link>
                </div>
              </CardBody>
            </Card>
          </div>
        </>
      ) : null}
    </>
  );
}
