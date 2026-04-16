"use client";

import { useState } from "react";
import { useParams, useRouter } from "next/navigation";
import Link from "next/link";
import { Card, CardBody } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { useMyActionDetail, useSubmitAcknowledgment } from "@/lib/user-portal/hooks";
import { UserActionRequirementStatus } from "@/lib/user-portal/types";
import { ActionType } from "@/lib/acknowledgments/types";

/**
 * Commitment Acknowledgment submission page (Sprint 6).
 * The user reads the commitment text, confirms understanding of the
 * policy, and accepts the commitment statement.
 */
export default function CommitmentAcknowledgmentPage() {
  const params = useParams();
  const router = useRouter();
  const requirementId = params.requirementId as string;
  const { data: action, isLoading } = useMyActionDetail(requirementId);
  const [policyConfirmed, setPolicyConfirmed] = useState(false);
  const [commitmentAccepted, setCommitmentAccepted] = useState(false);

  const submitMutation = useSubmitAcknowledgment({
    onSuccess: () => {
      router.push(`/actions/${requirementId}/confirmation`);
    },
  });

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

  // Guard: only commitment type, only open requirements
  const isOpen = action.status === UserActionRequirementStatus.Pending || action.status === UserActionRequirementStatus.Overdue;
  if (action.actionType !== ActionType.AcknowledgmentWithCommitment || !isOpen) {
    return (
      <Card className="p-6">
        <p className="text-sm text-[var(--color-text-secondary)]">هذا الإجراء لا يتطلب إقراراً مع تعهّد أو تم إكماله بالفعل.</p>
        <div className="mt-3">
          <Link href={`/actions/${requirementId}`}>
            <Button variant="secondary" size="sm">العودة</Button>
          </Link>
        </div>
      </Card>
    );
  }

  const canSubmit = policyConfirmed && commitmentAccepted;

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
        <h1 className="text-2xl font-bold">تأكيد الإقرار والتعهّد</h1>
        <p className="mt-1 text-[var(--color-text-secondary)]">{action.title}</p>
      </section>

      <div className="mx-auto max-w-2xl space-y-6">
        {/* Policy context */}
        <Card>
          <CardBody>
            <h2 className="mb-2 text-lg font-semibold">السياسة</h2>
            <p className="text-sm font-medium">{action.policyTitle}</p>
            {action.policySummary && (
              <p className="mt-1 text-sm text-[var(--color-text-secondary)]">{action.policySummary}</p>
            )}
            {action.hasPolicyDocument && (
              <div className="mt-3">
                <Link href={`/actions/${requirementId}/policy`}>
                  <Button variant="ghost" size="sm">عرض مستند السياسة</Button>
                </Link>
              </div>
            )}
          </CardBody>
        </Card>

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
        <Card>
          <CardBody>
            <h2 className="mb-3 text-lg font-semibold">نص التعهّد</h2>
            <div className="rounded-[10px] border border-[var(--color-border-default)] bg-[var(--color-surface-secondary)] p-4">
              <p className="whitespace-pre-wrap text-sm leading-relaxed">
                {action.commitmentText || "لم يتم تحديد نص التعهّد."}
              </p>
            </div>
          </CardBody>
        </Card>

        {/* Confirmations */}
        <Card>
          <CardBody>
            <div className="space-y-4">
              <div className="flex items-start gap-3">
                <input
                  type="checkbox"
                  id="confirm-policy"
                  checked={policyConfirmed}
                  onChange={(e) => setPolicyConfirmed(e.target.checked)}
                  className="mt-1 h-5 w-5 shrink-0 rounded border-[var(--color-border-default)]"
                />
                <label htmlFor="confirm-policy" className="text-sm leading-relaxed">
                  أقرّ بأنني قد اطّلعت على السياسة أعلاه وفهمت محتواها ومتطلباتها.
                </label>
              </div>

              <div className="flex items-start gap-3">
                <input
                  type="checkbox"
                  id="accept-commitment"
                  checked={commitmentAccepted}
                  onChange={(e) => setCommitmentAccepted(e.target.checked)}
                  className="mt-1 h-5 w-5 shrink-0 rounded border-[var(--color-border-default)]"
                />
                <label htmlFor="accept-commitment" className="text-sm leading-relaxed">
                  أوافق على نص التعهّد المذكور أعلاه وألتزم بمحتواه.
                </label>
              </div>
            </div>

            {submitMutation.error && (
              <p className="mt-3 text-sm text-red-600">
                {submitMutation.error.message || "حدث خطأ أثناء الإرسال. يرجى المحاولة مرة أخرى."}
              </p>
            )}

            <div className="mt-6 flex items-center justify-end gap-3">
              <Link href={`/actions/${requirementId}`}>
                <Button variant="secondary">إلغاء</Button>
              </Link>
              <Button
                disabled={!canSubmit || submitMutation.isPending}
                onClick={() => submitMutation.mutate(requirementId)}
              >
                {submitMutation.isPending ? "جاري الإرسال…" : "تأكيد الإقرار والتعهّد"}
              </Button>
            </div>
          </CardBody>
        </Card>
      </div>
    </>
  );
}
