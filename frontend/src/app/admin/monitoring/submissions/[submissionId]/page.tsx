"use client";

import Link from "next/link";
import { useParams } from "next/navigation";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";
import { useAdminSubmissionDetail } from "@/lib/admin/hooks";
import { actionTypeLabel } from "@/lib/acknowledgments/labels";
import { formatDateTime } from "@/lib/admin/labels";
import type { AdminFieldValueDto } from "@/lib/admin/types";

/**
 * Admin submission review page (Sprint 7).
 * Displays a complete view of a user submission including user context,
 * action/policy context, commitment text (if applicable), and field values
 * for form-based disclosures.
 * Per admin-portal-pages.md §16 (submission drill-down from requirement detail).
 */
export default function AdminSubmissionDetailPage() {
  const params = useParams();
  const submissionId = params.submissionId as string;

  const { data, isLoading, isError, error, refetch } = useAdminSubmissionDetail(submissionId);

  if (isLoading) {
    return <p className="text-sm text-[var(--color-text-tertiary)]">جاري تحميل التفاصيل…</p>;
  }

  if (isError) {
    return (
      <div className="space-y-3">
        <p className="text-sm text-red-700">
          تعذّر تحميل الإفادة: {error?.title ?? "خطأ غير معروف"}
        </p>
        <Button variant="secondary" onClick={() => refetch()}>إعادة المحاولة</Button>
      </div>
    );
  }

  if (!data) return null;

  return (
    <>
      <section className="mb-6">
        <div className="mb-2 flex items-center gap-3">
          {data.requirementId ? (
            <Link
              href={`/admin/monitoring/${data.requirementId}`}
              className="text-sm text-[var(--color-text-link)] hover:underline"
            >
              ← العودة لتفاصيل الإجراء
            </Link>
          ) : (
            <Link
              href="/admin/monitoring"
              className="text-sm text-[var(--color-text-link)] hover:underline"
            >
              ← العودة لقائمة المتابعة
            </Link>
          )}
        </div>
        <h1 className="text-2xl font-bold">مراجعة الإفادة المقدّمة</h1>
      </section>

      <div className="grid gap-6 lg:grid-cols-2">
        {/* Submission info */}
        <Card>
          <CardHeader>
            <CardTitle>بيانات الإفادة</CardTitle>
            {data.isLateSubmission ? (
              <Badge status="overdue">تقديم متأخر</Badge>
            ) : (
              <Badge status="completed">في الوقت المحدد</Badge>
            )}
          </CardHeader>
          <CardBody>
            <dl className="space-y-3 text-sm">
              <div className="flex justify-between">
                <dt className="text-[var(--color-text-tertiary)]">تاريخ التقديم</dt>
                <dd>{formatDateTime(data.submittedAtUtc)}</dd>
              </div>
              <div className="flex justify-between">
                <dt className="text-[var(--color-text-tertiary)]">نوع الإجراء</dt>
                <dd>{actionTypeLabel[data.actionType]}</dd>
              </div>
              {data.cycleReference ? (
                <div className="flex justify-between">
                  <dt className="text-[var(--color-text-tertiary)]">مرجع الدورة</dt>
                  <dd dir="ltr" className="font-mono text-xs">{data.cycleReference}</dd>
                </div>
              ) : null}
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
              {data.actionDescription ? (
                <div>
                  <dt className="text-[var(--color-text-tertiary)] mb-1">الوصف</dt>
                  <dd className="text-[var(--color-text-secondary)]">{data.actionDescription}</dd>
                </div>
              ) : null}
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
                <dd>{data.policyTitle}</dd>
              </div>
              <div className="flex justify-between">
                <dt className="text-[var(--color-text-tertiary)]">نسخة السياسة</dt>
                <dd>v{data.policyVersionNumber}</dd>
              </div>
              {data.policyVersionLabel ? (
                <div className="flex justify-between">
                  <dt className="text-[var(--color-text-tertiary)]">تسمية النسخة</dt>
                  <dd>{data.policyVersionLabel}</dd>
                </div>
              ) : null}
            </dl>
          </CardBody>
        </Card>
      </div>

      {/* Commitment text section (AcknowledgmentWithCommitment) */}
      {data.commitmentText ? (
        <div className="mt-6">
          <Card>
            <CardHeader>
              <CardTitle>نص التعهّد</CardTitle>
            </CardHeader>
            <CardBody>
              <p className="text-sm leading-relaxed whitespace-pre-wrap">
                {data.commitmentText}
              </p>
            </CardBody>
          </Card>
        </div>
      ) : null}

      {/* Field values section (FormBasedDisclosure) */}
      {data.fieldValues && data.fieldValues.length > 0 ? (
        <div className="mt-6">
          <Card>
            <CardHeader>
              <CardTitle>بيانات النموذج</CardTitle>
            </CardHeader>
            <CardBody>
              <div className="overflow-x-auto">
                <table className="w-full text-right text-sm">
                  <thead className="border-b border-[var(--color-border-default)] text-xs text-[var(--color-text-tertiary)]">
                    <tr>
                      <th className="py-2 pe-4 font-medium">الحقل</th>
                      <th className="py-2 pe-4 font-medium">النوع</th>
                      <th className="py-2 pe-4 font-medium">القيمة</th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-[var(--color-border-default)]">
                    {data.fieldValues.map((field) => (
                      <tr key={field.id}>
                        <td className="py-3 pe-4 font-medium">{field.fieldLabel}</td>
                        <td className="py-3 pe-4 text-xs text-[var(--color-text-tertiary)]">
                          {field.fieldType}
                        </td>
                        <td className="py-3 pe-4">{renderFieldValue(field)}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </CardBody>
          </Card>
        </div>
      ) : null}

      {/* Raw submission JSON (fallback / always available for audit) */}
      {data.submissionJson ? (
        <div className="mt-6">
          <Card>
            <CardHeader>
              <CardTitle>البيانات الخام</CardTitle>
            </CardHeader>
            <CardBody>
              <pre
                dir="ltr"
                className="max-h-64 overflow-auto rounded-lg bg-[var(--color-surface-subtle)] p-4 text-xs leading-relaxed"
              >
                {formatJson(data.submissionJson)}
              </pre>
            </CardBody>
          </Card>
        </div>
      ) : null}
    </>
  );
}

/** Render a field value based on its type, picking the first non-null typed value. */
function renderFieldValue(field: AdminFieldValueDto): string {
  if (field.valueBoolean !== null && field.valueBoolean !== undefined) {
    return field.valueBoolean ? "نعم" : "لا";
  }
  if (field.valueDate !== null && field.valueDate !== undefined) {
    return formatDateTime(field.valueDate);
  }
  if (field.valueNumber !== null && field.valueNumber !== undefined) {
    return String(field.valueNumber);
  }
  if (field.valueText !== null && field.valueText !== undefined) {
    return field.valueText;
  }
  if (field.valueJson !== null && field.valueJson !== undefined) {
    return field.valueJson;
  }
  return "—";
}

/** Pretty-print a JSON string; falls back to raw value on parse failure. */
function formatJson(raw: string): string {
  try {
    return JSON.stringify(JSON.parse(raw), null, 2);
  } catch {
    return raw;
  }
}
