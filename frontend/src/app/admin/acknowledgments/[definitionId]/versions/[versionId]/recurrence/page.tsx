"use client";

import Link from "next/link";
import { use, useState } from "react";
import { useRouter } from "next/navigation";
import { Button } from "@/components/ui/button";
import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";
import { RecurrenceForm } from "@/components/recurrence/RecurrenceForm";
import { RecurrenceSummary } from "@/components/recurrence/RecurrenceSummary";
import {
  useAcknowledgmentDefinition,
  useAcknowledgmentVersion,
  useSetAcknowledgmentVersionRecurrence,
} from "@/lib/acknowledgments/hooks";
import { AcknowledgmentVersionStatus } from "@/lib/acknowledgments/types";
import { useSession } from "@/lib/auth/SessionProvider";
import { Roles, hasAnyRole } from "@/lib/auth/roles";

interface Props {
  params: Promise<{ definitionId: string; versionId: string }>;
}

/**
 * Dedicated recurrence-configuration screen (BR-046). Writeable only on
 * drafts and only for AcknowledgmentManager/SystemAdministrator. Published
 * versions are immutable (BR-031) so the UI falls back to a read-only
 * summary.
 */
export default function RecurrencePage({ params }: Props) {
  const { definitionId, versionId } = use(params);
  const router = useRouter();

  const { user } = useSession();
  const canAuthor = hasAnyRole(user?.roles ?? [], [
    Roles.AcknowledgmentManager,
    Roles.SystemAdministrator,
  ]);

  const { data: definition } = useAcknowledgmentDefinition(definitionId);
  const {
    data: version,
    isLoading,
    isError,
    error,
    refetch,
  } = useAcknowledgmentVersion(definitionId, versionId);

  const setRecurrence = useSetAcknowledgmentVersionRecurrence(definitionId, versionId);
  const [banner, setBanner] = useState<string | null>(null);

  if (isLoading) {
    return <p className="text-sm text-[var(--color-text-tertiary)]">جاري التحميل…</p>;
  }
  if (isError || !version) {
    return (
      <Card>
        <CardBody>
          <p className="text-sm text-red-700">
            تعذّر تحميل النسخة: {error?.title ?? "غير متاح"}
          </p>
          <div className="mt-3">
            <Button variant="secondary" onClick={() => refetch()}>
              إعادة المحاولة
            </Button>
          </div>
        </CardBody>
      </Card>
    );
  }

  const isDraft = version.status === AcknowledgmentVersionStatus.Draft;
  const canEdit = canAuthor && isDraft;

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
        <h1 className="mt-2 text-3xl font-bold">نموذج التكرار</h1>
        <p className="mt-2 text-sm text-[var(--color-text-secondary)]">
          يتحكم نموذج التكرار في متى تُنشأ متطلبات الإقرار لكل موظف ضمن الجمهور
          المستهدف.
        </p>
      </section>

      {banner ? (
        <div
          role="status"
          className="mb-4 rounded-md border border-green-200 bg-green-50 px-3 py-2 text-sm text-green-800"
        >
          {banner}
        </div>
      ) : null}

      <Card>
        <CardHeader>
          <CardTitle>الإعدادات الحالية</CardTitle>
        </CardHeader>
        <CardBody>
          <RecurrenceSummary
            recurrenceModel={version.recurrenceModel}
            startDate={version.startDate}
            dueDate={version.dueDate}
          />
        </CardBody>
      </Card>

      <Card className="mt-4">
        <CardHeader>
          <CardTitle>{canEdit ? "تحديث نموذج التكرار" : "تفاصيل النموذج"}</CardTitle>
        </CardHeader>
        <CardBody>
          {canEdit ? (
            <RecurrenceForm
              initialValue={{
                recurrenceModel: version.recurrenceModel,
                startDate: version.startDate,
                dueDate: version.dueDate,
              }}
              onCancel={() =>
                router.push(`/admin/acknowledgments/${definitionId}/versions/${versionId}`)
              }
              onSubmit={async (input) => {
                await setRecurrence.mutateAsync(input);
                await refetch();
                setBanner("تم تحديث نموذج التكرار.");
              }}
            />
          ) : (
            <p className="text-sm text-[var(--color-text-tertiary)]">
              {isDraft
                ? "لا تملك الصلاحيات اللازمة لتعديل نموذج التكرار."
                : "النسخة المنشورة غير قابلة للتعديل (BR-031)."}
            </p>
          )}
        </CardBody>
      </Card>
    </>
  );
}
