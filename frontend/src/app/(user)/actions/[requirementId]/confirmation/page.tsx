"use client";

import { useParams } from "next/navigation";
import Link from "next/link";
import { Card, CardBody } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { RequirementStatusBadge } from "@/components/user-portal/RequirementStatusBadge";
import { useMyActionDetail } from "@/lib/user-portal/hooks";
import { actionTypeLabel } from "@/lib/acknowledgments/labels";
import { formatDateTimeAr } from "@/lib/user-portal/labels";

/**
 * Submission Confirmation page (Sprint 6). Displayed after the user
 * successfully submits an acknowledgment or disclosure. Shows a success
 * message, submission timestamp, and navigation options.
 */
export default function SubmissionConfirmationPage() {
  const params = useParams();
  const requirementId = params.requirementId as string;
  const { data: action, isLoading } = useMyActionDetail(requirementId);

  if (isLoading) {
    return (
      <Card className="p-6">
        <p className="text-sm text-[var(--color-text-tertiary)]">جاري التحميل…</p>
      </Card>
    );
  }

  if (!action) {
    return (
      <Card className="p-6">
        <p className="text-sm text-[var(--color-text-secondary)]">لم يتم العثور على الإجراء المطلوب.</p>
      </Card>
    );
  }

  return (
    <div className="mx-auto max-w-lg py-12">
      <Card>
        <CardBody>
          {/* Success icon */}
          <div className="mb-6 flex justify-center">
            <div className="flex h-16 w-16 items-center justify-center rounded-full bg-[var(--color-status-completed-bg)]">
              <svg
                className="h-8 w-8 text-[var(--color-status-completed-text)]"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
              </svg>
            </div>
          </div>

          <h1 className="mb-2 text-center text-2xl font-bold">تم الإرسال بنجاح</h1>
          <p className="mb-6 text-center text-[var(--color-text-secondary)]">
            تم تسجيل إجراؤك بنجاح في النظام.
          </p>

          {/* Details */}
          <div className="mb-6 space-y-3 rounded-[10px] bg-[var(--color-surface-subtle)] p-4 text-sm">
            <div className="flex justify-between">
              <span className="text-[var(--color-text-secondary)]">العنوان</span>
              <span className="font-medium">{action.title}</span>
            </div>
            <div className="flex justify-between">
              <span className="text-[var(--color-text-secondary)]">النوع</span>
              <span>{actionTypeLabel[action.actionType]}</span>
            </div>
            <div className="flex justify-between">
              <span className="text-[var(--color-text-secondary)]">السياسة</span>
              <span>{action.policyTitle}</span>
            </div>
            <div className="flex justify-between">
              <span className="text-[var(--color-text-secondary)]">الحالة</span>
              <RequirementStatusBadge status={action.status} />
            </div>
            {action.submittedAtUtc && (
              <div className="flex justify-between">
                <span className="text-[var(--color-text-secondary)]">تاريخ الإرسال</span>
                <span>{formatDateTimeAr(action.submittedAtUtc)}</span>
              </div>
            )}
          </div>

          {/* Navigation */}
          <div className="flex flex-col gap-3 sm:flex-row sm:justify-center">
            <Link href="/dashboard">
              <Button variant="primary" className="w-full sm:w-auto">
                العودة للوحة الرئيسية
              </Button>
            </Link>
            <Link href="/actions">
              <Button variant="secondary" className="w-full sm:w-auto">
                الإجراءات المطلوبة
              </Button>
            </Link>
            {action.submissionId && (
              <Link href={`/history/${action.submissionId}`}>
                <Button variant="ghost" className="w-full sm:w-auto">
                  عرض تفاصيل الإرسال
                </Button>
              </Link>
            )}
          </div>
        </CardBody>
      </Card>
    </div>
  );
}
