"use client";

import { useParams, useRouter } from "next/navigation";
import Link from "next/link";
import { Card, CardBody } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { DynamicFormRenderer } from "@/components/forms/DynamicFormRenderer";
import { useMyActionDetail, useSubmitDisclosure } from "@/lib/user-portal/hooks";
import { UserActionRequirementStatus } from "@/lib/user-portal/types";
import { ActionType } from "@/lib/acknowledgments/types";

/**
 * Form-Based Disclosure submission page (Sprint 6).
 * Renders the dynamic form from the form definition associated with the
 * acknowledgment version, validates inline, and submits structured data.
 */
export default function DisclosureSubmissionPage() {
  const params = useParams();
  const router = useRouter();
  const requirementId = params.requirementId as string;
  const { data: action, isLoading } = useMyActionDetail(requirementId);

  const submitMutation = useSubmitDisclosure({
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

  // Guard: only form-based disclosure, only open requirements
  const isOpen = action.status === UserActionRequirementStatus.Pending || action.status === UserActionRequirementStatus.Overdue;
  if (action.actionType !== ActionType.FormBasedDisclosure || !isOpen) {
    return (
      <Card className="p-6">
        <p className="text-sm text-[var(--color-text-secondary)]">هذا الإجراء لا يتطلب إفصاحاً عبر نموذج أو تم إكماله بالفعل.</p>
        <div className="mt-3">
          <Link href={`/actions/${requirementId}`}>
            <Button variant="secondary" size="sm">العودة</Button>
          </Link>
        </div>
      </Card>
    );
  }

  if (!action.formDefinition) {
    return (
      <Card className="p-6">
        <p className="text-sm text-[var(--color-text-secondary)]">لم يتم تهيئة نموذج الإفصاح لهذا الإجراء بعد.</p>
        <div className="mt-3">
          <Link href={`/actions/${requirementId}`}>
            <Button variant="secondary" size="sm">العودة</Button>
          </Link>
        </div>
      </Card>
    );
  }

  async function handleFormSubmit(values: Record<string, unknown>) {
    const submissionJson = JSON.stringify(values);
    await submitMutation.mutateAsync({ requirementId, submissionJson });
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
        <h1 className="text-2xl font-bold">نموذج الإفصاح</h1>
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

        {/* Dynamic form */}
        <Card>
          <CardBody>
            <h2 className="mb-4 text-lg font-semibold">تعبئة النموذج</h2>

            {submitMutation.error && (
              <div className="mb-4 rounded-[10px] border border-red-200 bg-red-50 p-3">
                <p className="text-sm text-red-600">
                  {submitMutation.error.message || "حدث خطأ أثناء الإرسال. يرجى المحاولة مرة أخرى."}
                </p>
              </div>
            )}

            <DynamicFormRenderer
              formDefinition={action.formDefinition}
              onSubmit={handleFormSubmit}
              disabled={submitMutation.isPending}
              submitLabel="إرسال الإفصاح"
            />
          </CardBody>
        </Card>

        {/* Cancel */}
        <div className="flex justify-start">
          <Link href={`/actions/${requirementId}`}>
            <Button variant="secondary">إلغاء</Button>
          </Link>
        </div>
      </div>
    </>
  );
}
