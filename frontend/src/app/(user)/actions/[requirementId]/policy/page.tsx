"use client";

import { useState } from "react";
import { useParams, useRouter } from "next/navigation";
import Link from "next/link";
import { Card, CardBody } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { useMyActionDetail } from "@/lib/user-portal/hooks";

/**
 * Policy Viewer page (Sprint 6). Displays the policy document associated
 * with the action requirement. Uses an iframe to render the PDF from the
 * existing backend endpoint.
 */
export default function PolicyViewerPage() {
  const params = useParams();
  const router = useRouter();
  const requirementId = params.requirementId as string;
  const { data: action, isLoading } = useMyActionDetail(requirementId);
  const [iframeError, setIframeError] = useState(false);

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

  const documentUrl = `/api/policies/${action.policyId}/versions/${action.policyVersionId}/document`;

  return (
    <>
      {/* Back link */}
      <div className="mb-4">
        <button
          onClick={() => router.back()}
          className="text-sm text-[var(--color-brand-primary)] hover:underline"
        >
          &larr; العودة لتفاصيل الإجراء
        </button>
      </div>

      <section className="mb-6">
        <h1 className="text-2xl font-bold">مستند السياسة</h1>
        <p className="mt-1 text-[var(--color-text-secondary)]">
          {action.policyTitle} — الإصدار {action.policyVersionNumber}
          {action.policyVersionLabel ? ` (${action.policyVersionLabel})` : ""}
        </p>
      </section>

      {!action.hasPolicyDocument ? (
        <Card>
          <CardBody>
            <p className="text-sm text-[var(--color-text-secondary)]">
              لا يوجد مستند مرفق لهذا الإصدار من السياسة.
            </p>
          </CardBody>
        </Card>
      ) : iframeError ? (
        <Card>
          <CardBody>
            <p className="mb-3 text-sm text-[var(--color-text-secondary)]">
              تعذّر عرض المستند مباشرة. يمكنك تحميله من الرابط أدناه.
            </p>
            <a href={documentUrl} target="_blank" rel="noopener noreferrer">
              <Button variant="secondary" size="sm">
                تحميل المستند
              </Button>
            </a>
          </CardBody>
        </Card>
      ) : (
        <div className="overflow-hidden rounded-[14px] border border-[var(--color-border-default)] bg-white shadow-[var(--shadow-sm)]">
          <iframe
            src={documentUrl}
            title={`مستند السياسة: ${action.policyTitle}`}
            className="h-[75vh] w-full"
            onError={() => setIframeError(true)}
          />
        </div>
      )}

      {/* Navigation back to action */}
      <div className="mt-6 flex items-center justify-between">
        <Link href={`/actions/${requirementId}`}>
          <Button variant="secondary">العودة لتفاصيل الإجراء</Button>
        </Link>
        {action.hasPolicyDocument && (
          <a href={documentUrl} target="_blank" rel="noopener noreferrer" download>
            <Button variant="ghost" size="sm">
              تحميل المستند
            </Button>
          </a>
        )}
      </div>
    </>
  );
}
