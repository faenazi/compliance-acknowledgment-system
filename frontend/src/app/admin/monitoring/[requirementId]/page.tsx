"use client";

import Link from "next/link";
import { useParams } from "next/navigation";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";
import { useAdminRequirementDetail } from "@/lib/admin/hooks";
import { actionTypeLabel } from "@/lib/acknowledgments/labels";
import {
  requirementStatusLabel,
  requirementStatusBadge,
  recurrenceModelLabel,
  formatDate,
  formatDateTime,
} from "@/lib/admin/labels";

/**
 * Admin requirement detail page (Sprint 7).
 * Shows full context of a user action requirement including user info,
 * action info, policy context, and linked submission if any.
 * Per admin-portal-pages.md §16 (requirement detail drill-down).
 */
export default function AdminRequirementDetailPage() {
  const params = useParams();
  const requirementId = params.requirementId as string;

  const { data, isLoading, isError, error, refetch } = useAdminRequirementDetail(requirementId);

  if (isLoading) {
    return <p className="text-sm text-[var(--color-text-tertiary)]">جاري تحميل التفاصيل…</p>;
  }

  if (isError) {
    return (
      <div className="space-y-3">
        <p className="text-sm text-red-700">
          تعذّر تحميل التفاصيل: {error?.title ?? "خطأ غير معروف"}
        </p>
        <Button variant="secondary" onClick={() => refetch()}>إعادة المحاولة</Button>
      </div>
    );
  }

  if (!data) return null;

  return (
    <>
      <section className="mb-6">
        <div className="mb-2">
          <Link href="/admin/monitoring" className="text-sm text-[var(--color-text-link)] hover:underline">
            ← العودة لقائمة المتابعة
          </Link>
        </div>
        <h1 className="text-2xl font-bold">تفاصيل الإجراء المطلوب</h1>
      </section>

      <div className="grid gap-6 lg:grid-cols-2">
        {/* Requirement info */}
        <Card>
          <CardHeader>
            <CardTitle>بيانات الإجراء</CardTitle>
            <Badge status={requirementStatusBadge[data.status]}>
              {requirementStatusLabel[data.status]}
            </Badge>
          </CardHeader>
          <CardBody>
            <dl className="space-y-3 text-sm">
              <div className="flex justify-between">
                <dt className="text-[var(--color-text-tertiary)]">تاريخ التكليف</dt>
                <dd>{formatDateTime(data.assignedAtUtc)}</dd>
              </div>
              <div className="flex justify-between">
                <dt className="text-[var(--color-text-tertiary)]">تاريخ الاستحقاق</dt>
                <dd>{formatDate(data.dueDate)}</dd>
              </div>
              <div className="flex justify-between">
                <dt className="text-[var(--color-text-tertiary)]">تاريخ الإنجاز</dt>
                <dd>{formatDateTime(data.completedAtUtc)}</dd>
              </div>
              <div className="flex justify-between">
                <dt className="text-[var(--color-text-tertiary)]">مرجع الدورة</dt>
                <dd dir="ltr" className="font-mono text-xs">{data.cycleReference}</dd>
              </div>
              <div className="flex justify-between">
                <dt className="text-[var(--color-text-tertiary)]">الدورة الحالية</dt>
                <dd>{data.isCurrent ? "نعم" : "لا"}</dd>
              </div>
            </dl>
          </CardBody>
        </Card>

        {/* User info */}
        <Card>
          <CardHeader>
            <CardTitle>بيانات المستخدم</CardTitle>
          </CardHeader>
          <CardBody>
            <dl className="space-y-3 text-sm">
              <div className="flex justify-between">
                <dt className="text-[var(--color-text-tertiary)]">الاسم</dt>
                <dd>{data.userDisplayName}</dd>
              </div>
              <div className="flex justify-between">
                <dt className="text-[var(--color-text-tertiary)]">الإدارة</dt>
                <dd>{data.userDepartment}</dd>
              </div>
              {data.userEmail ? (
                <div className="flex justify-between">
                  <dt className="text-[var(--color-text-tertiary)]">البريد الإلكتروني</dt>
                  <dd dir="ltr">{data.userEmail}</dd>
                </div>
              ) : null}
            </dl>
          </CardBody>
        </Card>

        {/* Action info */}
        <Card>
          <CardHeader>
            <CardTitle>تفاصيل الإقرار</CardTitle>
          </CardHeader>
          <CardBody>
            <dl className="space-y-3 text-sm">
              <div className="flex justify-between">
                <dt className="text-[var(--color-text-tertiary)]">العنوان</dt>
                <dd>
                  <Link
                    href={`/admin/acknowledgments/${data.acknowledgmentDefinitionId}`}
                    className="text-[var(--color-text-link)] hover:underline"
                  >
                    {data.actionTitle}
                  </Link>
                </dd>
              </div>
              <div className="flex justify-between">
                <dt className="text-[var(--color-text-tertiary)]">نوع الإجراء</dt>
                <dd>{actionTypeLabel[data.actionType]}</dd>
              </div>
              <div className="flex justify-between">
                <dt className="text-[var(--color-text-tertiary)]">نموذج التكرار</dt>
                <dd>{recurrenceModelLabel[data.recurrenceModel]}</dd>
              </div>
              <div className="flex justify-between">
                <dt className="text-[var(--color-text-tertiary)]">النسخة</dt>
                <dd>v{data.versionNumber}</dd>
              </div>
              <div className="flex justify-between">
                <dt className="text-[var(--color-text-tertiary)]">الإدارة المالكة</dt>
                <dd>{data.ownerDepartment}</dd>
              </div>
            </dl>
          </CardBody>
        </Card>

        {/* Policy info */}
        <Card>
          <CardHeader>
            <CardTitle>السياسة المرتبطة</CardTitle>
          </CardHeader>
          <CardBody>
            <dl className="space-y-3 text-sm">
              <div className="flex justify-between">
                <dt className="text-[var(--color-text-tertiary)]">عنوان السياسة</dt>
                <dd>
                  <Link
                    href={`/admin/policies/${data.policyId}`}
                    className="text-[var(--color-text-link)] hover:underline"
                  >
                    {data.policyTitle}
                  </Link>
                </dd>
              </div>
              <div className="flex justify-between">
                <dt className="text-[var(--color-text-tertiary)]">نسخة السياسة</dt>
                <dd>v{data.policyVersionNumber}</dd>
              </div>
            </dl>
          </CardBody>
        </Card>
      </div>

      {/* Submission section */}
      {data.submissionId ? (
        <div className="mt-6">
          <Card>
            <CardHeader>
              <CardTitle>الإفادة المقدّمة</CardTitle>
              {data.isLateSubmission ? (
                <Badge status="overdue">تقديم متأخر</Badge>
              ) : null}
            </CardHeader>
            <CardBody>
              <dl className="space-y-3 text-sm">
                <div className="flex justify-between">
                  <dt className="text-[var(--color-text-tertiary)]">تاريخ التقديم</dt>
                  <dd>{formatDateTime(data.submittedAtUtc)}</dd>
                </div>
              </dl>
              <div className="mt-4">
                <Link href={`/admin/monitoring/submissions/${data.submissionId}`}>
                  <Button variant="secondary" size="sm">عرض تفاصيل الإفادة</Button>
                </Link>
              </div>
            </CardBody>
          </Card>
        </div>
      ) : null}
    </>
  );
}
