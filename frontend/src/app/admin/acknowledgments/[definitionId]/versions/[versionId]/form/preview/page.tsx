"use client";

import Link from "next/link";
import { use, useState } from "react";
import { Button } from "@/components/ui/button";
import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";
import { DynamicFormRenderer } from "@/components/forms/DynamicFormRenderer";
import {
  useAcknowledgmentDefinition,
  useAcknowledgmentVersion,
} from "@/lib/acknowledgments/hooks";
import { useFormDefinition } from "@/lib/forms/hooks";

interface Props {
  params: Promise<{ definitionId: string; versionId: string }>;
}

/**
 * Preview page for the form definition. Renders the dynamic form in read-only
 * mode so admins can verify the layout before publishing (admin-portal-pages §13).
 */
export default function FormPreviewPage({ params }: Props) {
  const { definitionId, versionId } = use(params);

  const { data: definition } = useAcknowledgmentDefinition(definitionId);
  const { data: version } = useAcknowledgmentVersion(definitionId, versionId);
  const { data: formDef, isLoading } = useFormDefinition(definitionId, versionId);

  const [submitted, setSubmitted] = useState<Record<string, unknown> | null>(null);

  if (isLoading) {
    return <p className="text-sm text-[var(--color-text-tertiary)]">جاري التحميل…</p>;
  }

  if (!formDef || formDef.fields.length === 0) {
    return (
      <Card>
        <CardBody>
          <p className="text-sm text-[var(--color-text-tertiary)]">
            لا توجد حقول في تعريف النموذج. عد إلى صفحة التعريف لإضافة حقول.
          </p>
        </CardBody>
      </Card>
    );
  }

  return (
    <>
      <section className="mb-6">
        <div className="text-xs text-[var(--color-text-tertiary)]">
          <Link href="/admin/acknowledgments" className="hover:underline">الإقرارات</Link>
          <span className="mx-2">›</span>
          <Link href={`/admin/acknowledgments/${definitionId}`} className="hover:underline">
            {definition?.title ?? "تفاصيل الإقرار"}
          </Link>
          <span className="mx-2">›</span>
          <Link href={`/admin/acknowledgments/${definitionId}/versions/${versionId}`} className="hover:underline">
            النسخة v{version?.versionNumber}
          </Link>
          <span className="mx-2">›</span>
          <Link href={`/admin/acknowledgments/${definitionId}/versions/${versionId}/form`} className="hover:underline">
            تعريف النموذج
          </Link>
        </div>
        <h1 className="mt-2 text-3xl font-bold">معاينة النموذج</h1>
        <p className="mt-2 text-sm text-[var(--color-text-secondary)]">
          هذه معاينة لتعريف النموذج. البيانات لن تُحفظ — الهدف هو التحقق من التخطيط والتحقق قبل النشر.
        </p>
      </section>

      <Card>
        <CardHeader>
          <CardTitle>نموذج الإفصاح (معاينة)</CardTitle>
        </CardHeader>
        <CardBody>
          {submitted ? (
            <div className="space-y-3">
              <div className="rounded-md border border-green-200 bg-green-50 px-3 py-2 text-sm text-green-800">
                تم إرسال المعاينة بنجاح (لم يتم حفظ البيانات).
              </div>
              <pre
                dir="ltr"
                className="max-h-64 overflow-auto rounded-[10px] bg-[var(--color-surface-secondary)] p-3 text-xs"
              >
                {JSON.stringify(submitted, null, 2)}
              </pre>
              <Button variant="secondary" onClick={() => setSubmitted(null)}>
                إعادة المعاينة
              </Button>
            </div>
          ) : (
            <DynamicFormRenderer
              formDefinition={formDef}
              submitLabel="إرسال المعاينة"
              onSubmit={async (values) => {
                setSubmitted(values);
              }}
            />
          )}
        </CardBody>
      </Card>
    </>
  );
}
