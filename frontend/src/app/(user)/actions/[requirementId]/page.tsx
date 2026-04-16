"use client";

import { useParams, useRouter } from "next/navigation";
import Link from "next/link";
import { Card, CardBody } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { RequirementStatusBadge } from "@/components/user-portal/RequirementStatusBadge";
import { useMyActionDetail } from "@/lib/user-portal/hooks";
import { actionTypeLabel, actionTypeDescription } from "@/lib/acknowledgments/labels";
import { recurrenceModelLabel } from "@/lib/acknowledgments/recurrenceLabels";
import { formatDateAr, formatDateTimeAr } from "@/lib/user-portal/labels";
import { UserActionRequirementStatus } from "@/lib/user-portal/types";
import { ActionType } from "@/lib/acknowledgments/types";

/**
 * Action Detail page (Sprint 6). Shows full context for a single action
 * requirement with navigation to policy viewer and submission pages.
 */
export default function ActionDetailPage() {
  const params = useParams();
  const router = useRouter();
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

  const isOpen = action.status === UserActionRequirementStatus.Pending || action.status === UserActionRequirementStatus.Overdue;
  const isCompleted = action.status === UserActionRequirementStatus.Completed;
  const isOverdue = action.status === UserActionRequirementStatus.Overdue;

  function getSubmissionHref(): string {
    switch (action!.actionType) {
      case ActionType.SimpleAcknowledgment:
        return `/actions/${requirementId}/acknowledge`;
      case ActionType.AcknowledgmentWithCommitment:
        return `/actions/${requirementId}/commit`;
      case ActionType.FormBasedDisclosure:
        return `/actions/${requirementId}/disclose`;
      default:
        return `/actions/${requirementId}/acknowledge`;
    }
  }

  function getSubmissionLabel(): string {
    switch (action!.actionType) {
      case ActionType.SimpleAcknowledgment:
        return "تأكيد الإقرار";
      case ActionType.AcknowledgmentWithCommitment:
        return "تأكيد الإقرار والتعهّد";
      case ActionType.FormBasedDisclosure:
        return "تعبئة نموذج الإفصاح";
      default:
        return "إرسال";
    }
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

      {/* Header */}
      <section className="mb-6">
        <div className="flex flex-wrap items-start justify-between gap-4">
          <div>
            <h1 className="text-2xl font-bold">{action.title}</h1>
            {action.description && (
              <p className="mt-2 text-[var(--color-text-secondary)]">{action.description}</p>
            )}
          </div>
          <RequirementStatusBadge status={action.status} />
        </div>
      </section>

      <div className="grid gap-6 lg:grid-cols-3">
        {/* Main content */}
        <div className="space-y-6 lg:col-span-2">
          {/* Overdue warning */}
          {isOverdue && (
            <div className="rounded-[10px] border border-[var(--color-status-overdue-border)] bg-[var(--color-status-overdue-bg)] p-4">
              <p className="text-sm font-medium text-[var(--color-status-overdue-text)]">
                هذا الإجراء متأخر عن موعد الاستحقاق. يرجى إكماله في أقرب وقت ممكن.
              </p>
            </div>
          )}

          {/* Summary */}
          {action.summary && (
            <Card>
              <CardBody>
                <h2 className="mb-2 text-lg font-semibold">ملخّص</h2>
                <p className="whitespace-pre-wrap text-sm">{action.summary}</p>
              </CardBody>
            </Card>
          )}

          {/* Commitment text */}
          {action.commitmentText && action.actionType === ActionType.AcknowledgmentWithCommitment && (
            <Card>
              <CardBody>
                <h2 className="mb-2 text-lg font-semibold">نص التعهّد</h2>
                <div className="rounded-[10px] bg-[var(--color-surface-secondary)] p-4">
                  <p className="whitespace-pre-wrap text-sm leading-relaxed">{action.commitmentText}</p>
                </div>
              </CardBody>
            </Card>
          )}

          {/* Policy info */}
          <Card>
            <CardBody>
              <h2 className="mb-3 text-lg font-semibold">السياسة المرتبطة</h2>
              <div className="space-y-2 text-sm">
                <div className="flex justify-between">
                  <span className="text-[var(--color-text-secondary)]">اسم السياسة</span>
                  <span className="font-medium">{action.policyTitle}</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-[var(--color-text-secondary)]">رقم الإصدار</span>
                  <span>{action.policyVersionNumber}{action.policyVersionLabel ? ` — ${action.policyVersionLabel}` : ""}</span>
                </div>
                {action.policySummary && (
                  <p className="mt-2 text-[var(--color-text-secondary)]">{action.policySummary}</p>
                )}
              </div>
              {action.hasPolicyDocument && (
                <div className="mt-4">
                  <Link href={`/actions/${requirementId}/policy`}>
                    <Button variant="secondary" size="sm">
                      عرض مستند السياسة
                    </Button>
                  </Link>
                </div>
              )}
            </CardBody>
          </Card>

          {/* Already submitted */}
          {isCompleted && action.submissionId && (
            <Card>
              <CardBody>
                <h2 className="mb-2 text-lg font-semibold">تم الإكمال</h2>
                <p className="text-sm text-[var(--color-text-secondary)]">
                  تم إرسال هذا الإجراء بتاريخ {formatDateTimeAr(action.submittedAtUtc)}.
                </p>
                <div className="mt-3">
                  <Link href={`/history/${action.submissionId}`}>
                    <Button variant="secondary" size="sm">
                      عرض تفاصيل الإرسال
                    </Button>
                  </Link>
                </div>
              </CardBody>
            </Card>
          )}
        </div>

        {/* Sidebar */}
        <div className="space-y-6">
          {/* Action metadata */}
          <Card>
            <CardBody>
              <h3 className="mb-3 text-base font-semibold">تفاصيل الإجراء</h3>
              <dl className="space-y-3 text-sm">
                <div>
                  <dt className="text-[var(--color-text-tertiary)]">النوع</dt>
                  <dd className="font-medium">{actionTypeLabel[action.actionType]}</dd>
                  <dd className="text-xs text-[var(--color-text-tertiary)]">{actionTypeDescription[action.actionType]}</dd>
                </div>
                <div>
                  <dt className="text-[var(--color-text-tertiary)]">التكرار</dt>
                  <dd className="font-medium">{recurrenceModelLabel(action.recurrenceModel)}</dd>
                </div>
                <div>
                  <dt className="text-[var(--color-text-tertiary)]">الدورة</dt>
                  <dd className="font-medium">{action.cycleReference}</dd>
                </div>
                <div>
                  <dt className="text-[var(--color-text-tertiary)]">القسم المسؤول</dt>
                  <dd className="font-medium">{action.ownerDepartment}</dd>
                </div>
                {action.dueDate && (
                  <div>
                    <dt className="text-[var(--color-text-tertiary)]">تاريخ الاستحقاق</dt>
                    <dd className={`font-medium ${isOverdue ? "text-[var(--color-status-overdue-text)]" : ""}`}>
                      {formatDateAr(action.dueDate)}
                    </dd>
                  </div>
                )}
                <div>
                  <dt className="text-[var(--color-text-tertiary)]">تاريخ التعيين</dt>
                  <dd>{formatDateTimeAr(action.assignedAtUtc)}</dd>
                </div>
                {action.completedAtUtc && (
                  <div>
                    <dt className="text-[var(--color-text-tertiary)]">تاريخ الإكمال</dt>
                    <dd>{formatDateTimeAr(action.completedAtUtc)}</dd>
                  </div>
                )}
              </dl>
            </CardBody>
          </Card>

          {/* Action button */}
          {isOpen && (
            <Link href={getSubmissionHref()}>
              <Button className="w-full" size="lg">
                {getSubmissionLabel()}
              </Button>
            </Link>
          )}
        </div>
      </div>
    </>
  );
}
