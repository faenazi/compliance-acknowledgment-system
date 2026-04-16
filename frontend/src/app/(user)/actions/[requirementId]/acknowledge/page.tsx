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
 * Simple Acknowledgment submission page (Sprint 6).
 * The user reviews the policy summary and confirms they have read
 * and understood it. No additional payload required.
 */
export default function SimpleAcknowledgmentPage() {
  const params = useParams();
  const router = useRouter();
  const requirementId = params.requirementId as string;
  const { data: action, isLoading } = useMyActionDetail(requirementId);
  const [confirmed, setConfirmed] = useState(false);

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

  // Guard: only simple acknowledgment, only open requirements
  const isOpen = action.status === UserActionRequirementStatus.Pending || action.status === UserActionRequirementStatus.Overdue;
  if (action.actionType !== ActionType.SimpleAcknowledgment || !isOpen) {
    return (
      <Card className="p-6">
        <p className="text-sm text-[var(--color-text-secondary)]">هذا الإجراء لا يتطلب إقراراً بسيطاً أو تم إكماله بالفعل.</p>
        <div className="mt-3">
          <Link href={`/actions/${requirementId}`}>
            <Button variant="secondary" size="sm">العودة</Button>
          </Link>
        </div>
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
        <h1 className="text-2xl font-bold">تأكيد الإقرار</h1>
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

        {/* Confirmation */}
        <Card>
          <CardBody>
            <div className="flex items-start gap-3">
              <input
                type="checkbox"
                id="confirm-ack"
                checked={confirmed}
                onChange={(e) => setConfirmed(e.target.checked)}
                className="mt-1 h-5 w-5 shrink-0 rounded border-[var(--color-border-default)]"
              />
              <label htmlFor="confirm-ack" className="text-sm leading-relaxed">
                أقرّ بأنني قد اطّلعت على السياسة أعلاه وفهمت محتواها ومتطلباتها.
              </label>
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
                disabled={!confirmed || submitMutation.isPending}
                onClick={() => submitMutation.mutate(requirementId)}
              >
                {submitMutation.isPending ? "جاري الإرسال…" : "تأكيد الإقرار"}
              </Button>
            </div>
          </CardBody>
        </Card>
      </div>
    </>
  );
}
