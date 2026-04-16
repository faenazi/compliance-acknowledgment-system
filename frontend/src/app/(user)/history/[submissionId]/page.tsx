"use client";

import { useParams, useRouter } from "next/navigation";
import Link from "next/link";
import { Card, CardBody } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { useMySubmissionDetail } from "@/lib/user-portal/hooks";
import { actionTypeLabel } from "@/lib/acknowledgments/labels";
import { formatDateTimeAr } from "@/lib/user-portal/labels";
import { ActionType } from "@/lib/acknowledgments/types";
import type { SubmissionFieldValueDto } from "@/lib/forms/types";
import { FormFieldType } from "@/lib/forms/types";

/**
 * Submission Detail page (Sprint 6). Read-only view of a past submission,
 * showing the context (policy, acknowledgment) and submitted data.
 */
export default function SubmissionDetailPage() {
  const params = useParams();
  const router = useRouter();
  const submissionId = params.submissionId as string;
  const { data: submission, isLoading } = useMySubmissionDetail(submissionId);

  if (isLoading) {
    return (
      <Card className="p-6">
        <p className="text-sm text-[var(--color-text-tertiary)]">جاري التحميل…</p>
      </Card>
    );
  }

  if (!submission) {
    return (
      <Card className="p-6">
        <p className="text-sm text-[var(--color-text-secondary)]">لم يتم العثور على الإرسال المطلوب.</p>
      </Card>
    );
  }

  return (
    <>
      {/* Back link */}
      <div className="mb-4">
        <button
          onClick={() => router.back()}
          className="text-sm text-[var(--color-brand-primary)] hover:underline"
        >
          &larr; العودة
        </button>
      </div>

      <section className="mb-6">
        <div className="flex flex-wrap items-start justify-between gap-4">
          <div>
            <h1 className="text-2xl font-bold">تفاصيل الإرسال</h1>
            <p className="mt-1 text-[var(--color-text-secondary)]">{submission.title}</p>
          </div>
          <div className="flex items-center gap-2">
            <Badge status="completed">مكتمل</Badge>
            {submission.isLateSubmission && (
              <Badge status="overdue">إرسال متأخر</Badge>
            )}
          </div>
        </div>
      </section>

      <div className="grid gap-6 lg:grid-cols-3">
        {/* Main content */}
        <div className="space-y-6 lg:col-span-2">
          {/* Commitment text (if applicable) */}
          {submission.commitmentText && submission.actionType === ActionType.AcknowledgmentWithCommitment && (
            <Card>
              <CardBody>
                <h2 className="mb-2 text-lg font-semibold">نص التعهّد</h2>
                <div className="rounded-[10px] bg-[var(--color-surface-secondary)] p-4">
                  <p className="whitespace-pre-wrap text-sm leading-relaxed">{submission.commitmentText}</p>
                </div>
              </CardBody>
            </Card>
          )}

          {/* Form field values (for form-based disclosure) */}
          {submission.actionType === ActionType.FormBasedDisclosure && submission.fieldValues && submission.fieldValues.length > 0 && (
            <Card>
              <CardBody>
                <h2 className="mb-4 text-lg font-semibold">بيانات الإفصاح</h2>
                <div className="space-y-3">
                  {submission.fieldValues.map((fv) => (
                    <FieldValueRow key={fv.id} field={fv} />
                  ))}
                </div>
              </CardBody>
            </Card>
          )}

          {/* Raw submission (for simple/commitment where no field values) */}
          {submission.actionType !== ActionType.FormBasedDisclosure && (
            <Card>
              <CardBody>
                <h2 className="mb-2 text-lg font-semibold">بيانات الإرسال</h2>
                <SubmissionJsonDisplay json={submission.submissionJson} />
              </CardBody>
            </Card>
          )}
        </div>

        {/* Sidebar */}
        <div className="space-y-6">
          <Card>
            <CardBody>
              <h3 className="mb-3 text-base font-semibold">معلومات الإرسال</h3>
              <dl className="space-y-3 text-sm">
                <div>
                  <dt className="text-[var(--color-text-tertiary)]">النوع</dt>
                  <dd className="font-medium">{actionTypeLabel[submission.actionType]}</dd>
                </div>
                <div>
                  <dt className="text-[var(--color-text-tertiary)]">السياسة</dt>
                  <dd className="font-medium">{submission.policyTitle}</dd>
                </div>
                <div>
                  <dt className="text-[var(--color-text-tertiary)]">إصدار السياسة</dt>
                  <dd>v{submission.policyVersionNumber}{submission.policyVersionLabel ? ` — ${submission.policyVersionLabel}` : ""}</dd>
                </div>
                <div>
                  <dt className="text-[var(--color-text-tertiary)]">القسم المسؤول</dt>
                  <dd className="font-medium">{submission.ownerDepartment}</dd>
                </div>
                <div>
                  <dt className="text-[var(--color-text-tertiary)]">تاريخ الإرسال</dt>
                  <dd>{formatDateTimeAr(submission.submittedAtUtc)}</dd>
                </div>
                {submission.description && (
                  <div>
                    <dt className="text-[var(--color-text-tertiary)]">الوصف</dt>
                    <dd className="text-[var(--color-text-secondary)]">{submission.description}</dd>
                  </div>
                )}
              </dl>
            </CardBody>
          </Card>

          {/* Navigation */}
          <div className="space-y-2">
            <Link href={`/actions/${submission.requirementId}`}>
              <Button variant="secondary" className="w-full">عرض تفاصيل الإجراء</Button>
            </Link>
            <Link href="/history">
              <Button variant="ghost" className="w-full">العودة للسجل</Button>
            </Link>
          </div>
        </div>
      </div>
    </>
  );
}

function FieldValueRow({ field }: { field: SubmissionFieldValueDto }) {
  return (
    <div className="flex flex-col gap-1 border-b border-[var(--color-border-soft)] pb-3 last:border-0 last:pb-0">
      <span className="text-xs text-[var(--color-text-tertiary)]">{field.fieldLabel}</span>
      <span className="text-sm font-medium">{formatFieldValue(field)}</span>
    </div>
  );
}

function formatFieldValue(field: SubmissionFieldValueDto): string {
  if (field.fieldType === FormFieldType.Checkbox || field.fieldType === FormFieldType.YesNo) {
    return field.valueBoolean ? "نعم" : "لا";
  }
  if (field.fieldType === FormFieldType.Date) {
    return field.valueDate ?? "—";
  }
  if (field.fieldType === FormFieldType.Number || field.fieldType === FormFieldType.Decimal) {
    return field.valueNumber?.toString() ?? "—";
  }
  if (field.fieldType === FormFieldType.MultiSelect && field.valueJson) {
    try {
      const arr = JSON.parse(field.valueJson) as string[];
      return arr.join("، ");
    } catch {
      return field.valueJson;
    }
  }
  return field.valueText ?? "—";
}

function SubmissionJsonDisplay({ json }: { json: string }) {
  try {
    const parsed = JSON.parse(json) as Record<string, unknown>;
    return (
      <div className="rounded-[10px] bg-[var(--color-surface-subtle)] p-4 text-sm">
        <dl className="space-y-2">
          {parsed.type && (
            <div className="flex justify-between">
              <dt className="text-[var(--color-text-tertiary)]">نوع الإقرار</dt>
              <dd className="font-medium">
                {parsed.type === "simple" ? "إقرار بسيط" : parsed.type === "commitment" ? "إقرار مع تعهّد" : String(parsed.type)}
              </dd>
            </div>
          )}
          {parsed.confirmed !== undefined && (
            <div className="flex justify-between">
              <dt className="text-[var(--color-text-tertiary)]">التأكيد</dt>
              <dd className="font-medium">{parsed.confirmed ? "نعم" : "لا"}</dd>
            </div>
          )}
          {parsed.timestamp && (
            <div className="flex justify-between">
              <dt className="text-[var(--color-text-tertiary)]">الطابع الزمني</dt>
              <dd>{formatDateTimeAr(parsed.timestamp as string)}</dd>
            </div>
          )}
        </dl>
      </div>
    );
  } catch {
    return (
      <pre className="overflow-x-auto rounded-[10px] bg-[var(--color-surface-subtle)] p-4 text-xs" dir="ltr">
        {json}
      </pre>
    );
  }
}
