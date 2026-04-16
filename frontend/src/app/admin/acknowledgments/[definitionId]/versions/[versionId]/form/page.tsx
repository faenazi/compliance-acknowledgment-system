"use client";

import Link from "next/link";
import { use, useState } from "react";
import { Button } from "@/components/ui/button";
import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";
import { FormDefinitionEditor } from "@/components/forms/FormDefinitionEditor";
import {
  useAcknowledgmentDefinition,
  useAcknowledgmentVersion,
} from "@/lib/acknowledgments/hooks";
import { AcknowledgmentVersionStatus } from "@/lib/acknowledgments/types";
import { useFormDefinition, useConfigureFormDefinition } from "@/lib/forms/hooks";
import type { FormFieldInput } from "@/lib/forms/types";
import { useSession } from "@/lib/auth/SessionProvider";
import { Roles, hasAnyRole } from "@/lib/auth/roles";

interface Props {
  params: Promise<{ definitionId: string; versionId: string }>;
}

/**
 * Form Definition Management page (admin-portal-pages §13).
 * Structured editor — not a visual drag-and-drop builder (BR-161).
 */
export default function FormDefinitionPage({ params }: Props) {
  const { definitionId, versionId } = use(params);

  const { user } = useSession();
  const canAuthor = hasAnyRole(user?.roles ?? [], [
    Roles.AcknowledgmentManager,
    Roles.SystemAdministrator,
  ]);

  const { data: definition } = useAcknowledgmentDefinition(definitionId);
  const {
    data: version,
    isLoading: versionLoading,
    isError: versionError,
    error: versionErr,
    refetch: refetchVersion,
  } = useAcknowledgmentVersion(definitionId, versionId);

  const {
    data: formDef,
    isLoading: formLoading,
    refetch: refetchForm,
  } = useFormDefinition(definitionId, versionId);

  const configureMutation = useConfigureFormDefinition(definitionId, versionId);

  const [banner, setBanner] = useState<string | null>(null);

  if (versionLoading || formLoading) {
    return <p className="text-sm text-[var(--color-text-tertiary)]">جاري التحميل…</p>;
  }

  if (versionError || !version) {
    return (
      <Card>
        <CardBody>
          <p className="text-sm text-red-700">
            تعذّر تحميل النسخة: {versionErr?.title ?? "غير متاح"}
          </p>
          <div className="mt-3">
            <Button variant="secondary" onClick={() => refetchVersion()}>
              إعادة المحاولة
            </Button>
          </div>
        </CardBody>
      </Card>
    );
  }

  const isDraft = version.status === AcknowledgmentVersionStatus.Draft;
  const canEdit = canAuthor && isDraft;

  const initialFields: FormFieldInput[] = (formDef?.fields ?? []).map((f) => ({
    fieldKey: f.fieldKey,
    label: f.label,
    fieldType: f.fieldType,
    isRequired: f.isRequired,
    sectionKey: f.sectionKey,
    helpText: f.helpText,
    placeholder: f.placeholder,
    displayText: f.displayText,
    options: f.options.length > 0 ? f.options : null,
  }));

  return (
    <>
      <section className="mb-6">
        <div className="text-xs text-[var(--color-text-tertiary)]">
          <Link href="/admin/acknowledgments" className="hover:underline">
            الإقرارات
          </Link>
          <span className="mx-2">›</span>
          <Link
            href={`/admin/acknowledgments/${definitionId}`}
            className="hover:underline"
          >
            {definition?.title ?? "تفاصيل الإقرار"}
          </Link>
          <span className="mx-2">›</span>
          <Link
            href={`/admin/acknowledgments/${definitionId}/versions/${versionId}`}
            className="hover:underline"
          >
            النسخة v{version.versionNumber}
          </Link>
        </div>
        <h1 className="mt-2 text-3xl font-bold">تعريف النموذج</h1>
        <p className="mt-2 text-sm text-[var(--color-text-secondary)]">
          حدد الحقول المطلوبة لنموذج الإفصاح. كل حقل يحتاج إلى مفتاح فريد
          ونوع وتسمية. الحقول المطلوبة ستُطلب قبل الإرسال (BR-074).
        </p>
      </section>

      {banner && (
        <div
          role="status"
          className="mb-4 rounded-md border border-green-200 bg-green-50 px-3 py-2 text-sm text-green-800"
        >
          {banner}
        </div>
      )}

      {!isDraft && (
        <div className="mb-4 rounded-md border border-yellow-200 bg-yellow-50 px-3 py-2 text-sm text-yellow-800">
          هذه النسخة ليست مسودة — تعريف النموذج للقراءة فقط (BR-004 / BR-080).
        </div>
      )}

      <Card className="mb-4">
        <CardHeader>
          <CardTitle>الحقول</CardTitle>
          {formDef && (
            <span className="text-xs text-[var(--color-text-tertiary)]">
              {formDef.fields.length} حقل — إصدار المخطط: {formDef.schemaVersion}
            </span>
          )}
        </CardHeader>
        <CardBody>
          <FormDefinitionEditor
            initialFields={initialFields}
            disabled={!canEdit}
            onSave={async (fields) => {
              await configureMutation.mutateAsync({ fields });
              await refetchForm();
              setBanner("تم حفظ تعريف النموذج بنجاح.");
            }}
          />
        </CardBody>
      </Card>

      {canEdit && formDef && formDef.fields.length > 0 && (
        <div className="flex justify-end">
          <Link
            href={`/admin/acknowledgments/${definitionId}/versions/${versionId}/form/preview`}
            className="text-sm text-[var(--color-brand-primary)] hover:underline"
          >
            معاينة النموذج ←
          </Link>
        </div>
      )}
    </>
  );
}
