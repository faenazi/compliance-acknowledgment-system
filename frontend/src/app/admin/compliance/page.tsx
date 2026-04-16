"use client";

import { useState } from "react";
import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { KpiCard } from "@/components/admin/KpiCard";
import { Badge } from "@/components/ui/badge";
import { useComplianceDashboard } from "@/lib/compliance/hooks";
import { formatDate } from "@/lib/admin/labels";
import { requirementStatusLabel, requirementStatusBadge } from "@/lib/admin/labels";
import { UserActionRequirementStatus } from "@/lib/requirements/types";
import Link from "next/link";
import {
  Users,
  Building2,
  ClipboardCheck,
  AlertTriangle,
  TrendingUp,
  CheckCircle,
  Clock,
  Download,
} from "lucide-react";

/**
 * Compliance Dashboard page (Sprint 8, admin-portal-pages §17).
 * Shows KPI cards, department completion, action completion,
 * and top non-compliant users.
 */
export default function ComplianceDashboardPage() {
  const [department, setDepartment] = useState("");
  const { data, isLoading, isError, error, refetch } = useComplianceDashboard({
    department: department || undefined,
  });

  return (
    <>
      <section className="mb-6 flex items-start justify-between gap-4">
        <div>
          <h1 className="text-3xl font-bold">لوحة الامتثال</h1>
          <p className="mt-2 text-[var(--color-text-secondary)]">
            ملخص حالة الامتثال عبر الإدارات والإجراءات.
          </p>
        </div>
        <div className="flex gap-2">
          <Link href="/admin/compliance/reports">
            <Button variant="secondary" size="sm">التقارير التفصيلية</Button>
          </Link>
        </div>
      </section>

      {/* Department filter */}
      <div className="mb-6 flex gap-3">
        <input
          type="text"
          placeholder="تصفية حسب القسم..."
          value={department}
          onChange={(e) => setDepartment(e.target.value)}
          className="h-[44px] rounded-[10px] border border-[var(--color-border-default)] bg-white px-4 text-sm text-[var(--color-text-primary)] placeholder:text-[var(--color-text-disabled)] focus:border-[var(--color-brand-primary)] focus:outline-none"
        />
      </div>

      {isLoading ? (
        <p className="text-sm text-[var(--color-text-tertiary)]">جاري تحميل بيانات الامتثال…</p>
      ) : isError ? (
        <div className="space-y-3">
          <p className="text-sm text-red-700">
            تعذّر تحميل بيانات الامتثال: {error?.title ?? "خطأ غير معروف"}
          </p>
          <Button variant="secondary" onClick={() => refetch()}>إعادة المحاولة</Button>
        </div>
      ) : data ? (
        <>
          {/* KPI cards */}
          <div className="mb-8 grid gap-4 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-5">
            <KpiCard
              label="إجمالي المتطلبات"
              value={data.totalRequirements}
              icon={<ClipboardCheck size={20} />}
            />
            <KpiCard
              label="مكتملة"
              value={data.completedRequirements}
              icon={<CheckCircle size={20} />}
              accentClass="text-[#5C6B1E]"
            />
            <KpiCard
              label="معلّقة"
              value={data.pendingRequirements}
              icon={<Clock size={20} />}
              accentClass="text-[var(--color-brand-primary)]"
            />
            <KpiCard
              label="متأخرة"
              value={data.overdueRequirements}
              icon={<AlertTriangle size={20} />}
              accentClass={data.overdueRequirements > 0 ? "text-[#9A3412]" : undefined}
            />
            <KpiCard
              label="نسبة الإنجاز"
              value={`${data.completionRate}%`}
              icon={<TrendingUp size={20} />}
              subtitle={`${data.completedRequirements} من ${data.totalRequirements}`}
            />
          </div>

          <div className="grid gap-6 lg:grid-cols-2">
            {/* Compliance by Department */}
            <Card>
              <CardHeader>
                <div className="flex items-center justify-between">
                  <CardTitle className="flex items-center gap-2">
                    <Building2 size={18} />
                    الامتثال حسب القسم
                  </CardTitle>
                  <a
                    href={`${process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:5100"}/api/admin/compliance/export/departments`}
                    target="_blank"
                    rel="noopener noreferrer"
                  >
                    <Button variant="ghost" size="sm"><Download size={14} className="ml-1" />تصدير</Button>
                  </a>
                </div>
              </CardHeader>
              <CardBody>
                {data.complianceByDepartment.length === 0 ? (
                  <p className="text-sm text-[var(--color-text-tertiary)]">لا توجد بيانات.</p>
                ) : (
                  <div className="overflow-x-auto">
                    <table className="w-full text-sm">
                      <thead>
                        <tr className="border-b border-[var(--color-border-default)] bg-[var(--color-surface-subtle)]">
                          <th className="px-3 py-2 text-right font-medium">القسم</th>
                          <th className="px-3 py-2 text-right font-medium">مكتمل</th>
                          <th className="px-3 py-2 text-right font-medium">معلّق</th>
                          <th className="px-3 py-2 text-right font-medium">متأخر</th>
                          <th className="px-3 py-2 text-right font-medium">النسبة</th>
                        </tr>
                      </thead>
                      <tbody>
                        {data.complianceByDepartment.map((d) => (
                          <tr key={d.department} className="border-b border-[var(--color-border-soft)]">
                            <td className="px-3 py-2 font-medium">{d.department}</td>
                            <td className="px-3 py-2 text-[#5C6B1E]">{d.completed}</td>
                            <td className="px-3 py-2">{d.pending}</td>
                            <td className="px-3 py-2 text-[#9A3412]">{d.overdue}</td>
                            <td className="px-3 py-2">
                              <div className="flex items-center gap-2">
                                <div className="h-2 w-16 overflow-hidden rounded-full bg-[var(--color-border-default)]">
                                  <div
                                    className="h-full rounded-full bg-[var(--color-brand-primary)]"
                                    style={{ width: `${Math.min(d.completionRate, 100)}%` }}
                                  />
                                </div>
                                <span className="text-xs">{d.completionRate}%</span>
                              </div>
                            </td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                )}
              </CardBody>
            </Card>

            {/* Compliance by Action */}
            <Card>
              <CardHeader>
                <div className="flex items-center justify-between">
                  <CardTitle className="flex items-center gap-2">
                    <ClipboardCheck size={18} />
                    الامتثال حسب الإجراء
                  </CardTitle>
                  <a
                    href={`${process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:5100"}/api/admin/compliance/export/actions`}
                    target="_blank"
                    rel="noopener noreferrer"
                  >
                    <Button variant="ghost" size="sm"><Download size={14} className="ml-1" />تصدير</Button>
                  </a>
                </div>
              </CardHeader>
              <CardBody>
                {data.complianceByAction.length === 0 ? (
                  <p className="text-sm text-[var(--color-text-tertiary)]">لا توجد بيانات.</p>
                ) : (
                  <div className="overflow-x-auto">
                    <table className="w-full text-sm">
                      <thead>
                        <tr className="border-b border-[var(--color-border-default)] bg-[var(--color-surface-subtle)]">
                          <th className="px-3 py-2 text-right font-medium">الإجراء</th>
                          <th className="px-3 py-2 text-right font-medium">مكتمل</th>
                          <th className="px-3 py-2 text-right font-medium">معلّق</th>
                          <th className="px-3 py-2 text-right font-medium">متأخر</th>
                          <th className="px-3 py-2 text-right font-medium">النسبة</th>
                        </tr>
                      </thead>
                      <tbody>
                        {data.complianceByAction.map((a) => (
                          <tr key={a.acknowledgmentDefinitionId} className="border-b border-[var(--color-border-soft)]">
                            <td className="px-3 py-2 font-medium">{a.actionTitle}</td>
                            <td className="px-3 py-2 text-[#5C6B1E]">{a.completed}</td>
                            <td className="px-3 py-2">{a.pending}</td>
                            <td className="px-3 py-2 text-[#9A3412]">{a.overdue}</td>
                            <td className="px-3 py-2">
                              <div className="flex items-center gap-2">
                                <div className="h-2 w-16 overflow-hidden rounded-full bg-[var(--color-border-default)]">
                                  <div
                                    className="h-full rounded-full bg-[#C0CB6C]"
                                    style={{ width: `${Math.min(a.completionRate, 100)}%` }}
                                  />
                                </div>
                                <span className="text-xs">{a.completionRate}%</span>
                              </div>
                            </td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                )}
              </CardBody>
            </Card>
          </div>

          {/* Top Non-Compliant Users */}
          <div className="mt-6">
            <Card>
              <CardHeader>
                <div className="flex items-center justify-between">
                  <CardTitle className="flex items-center gap-2">
                    <Users size={18} />
                    أكثر المستخدمين غير الممتثلين
                  </CardTitle>
                  <Link href="/admin/compliance/reports">
                    <Button variant="ghost" size="sm">عرض الكل</Button>
                  </Link>
                </div>
              </CardHeader>
              <CardBody>
                {data.topNonCompliantUsers.length === 0 ? (
                  <p className="text-sm text-[var(--color-text-tertiary)]">جميع المستخدمين ممتثلون.</p>
                ) : (
                  <div className="overflow-x-auto">
                    <table className="w-full text-sm">
                      <thead>
                        <tr className="border-b border-[var(--color-border-default)] bg-[var(--color-surface-subtle)]">
                          <th className="px-3 py-2 text-right font-medium">الاسم</th>
                          <th className="px-3 py-2 text-right font-medium">القسم</th>
                          <th className="px-3 py-2 text-right font-medium">معلّق</th>
                          <th className="px-3 py-2 text-right font-medium">متأخر</th>
                          <th className="px-3 py-2 text-right font-medium">الإجمالي</th>
                        </tr>
                      </thead>
                      <tbody>
                        {data.topNonCompliantUsers.map((u) => (
                          <tr key={u.userId} className="border-b border-[var(--color-border-soft)]">
                            <td className="px-3 py-2 font-medium">{u.displayName}</td>
                            <td className="px-3 py-2 text-[var(--color-text-secondary)]">{u.department}</td>
                            <td className="px-3 py-2">{u.pendingCount}</td>
                            <td className="px-3 py-2 text-[#9A3412] font-medium">{u.overdueCount}</td>
                            <td className="px-3 py-2 font-semibold">{u.totalNonCompliant}</td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                )}
              </CardBody>
            </Card>
          </div>
        </>
      ) : null}
    </>
  );
}
