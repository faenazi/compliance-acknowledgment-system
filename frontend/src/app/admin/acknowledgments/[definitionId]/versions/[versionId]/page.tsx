"use client";

import Link from "next/link";
import { use, useState } from "react";
import { useRouter } from "next/navigation";
import { Button } from "@/components/ui/button";
import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";
import {
  AcknowledgmentVersionStatusBadge,
} from "@/components/acknowledgments/AcknowledgmentStatusBadge";
import { ActionTypeBadge } from "@/components/acknowledgments/ActionTypeBadge";
import { AcknowledgmentVersionForm } from "@/components/acknowledgments/AcknowledgmentVersionForm";
import { RecurrenceSummary } from "@/components/recurrence/RecurrenceSummary";
import {
  useAcknowledgmentDefinition,
  useAcknowledgmentVersion,
  useArchiveAcknowledgmentVersion,
  usePublishAcknowledgmentVersion,
  useUpdateAcknowledgmentVersionDraft,
} from "@/lib/acknowledgments/hooks";
import {
  ActionType,
  AcknowledgmentVersionStatus,
  type UpdateAcknowledgmentVersionDraftInput,
} from "@/lib/acknowledgments/types";
import { actionTypeLabel } from "@/lib/acknowledgments/labels";
import { useSession } from "@/lib/auth/SessionProvider";
import { Roles, hasAnyRole } from "@/lib/auth/roles";
import type { ApiError } from "@/lib/api/client";

interface Props {
  params: Promise<{ definitionId: string; versionId: string }>;
}

/**
 * Acknowledgment version detail page.
 * <ul>
 *   <li>Draft: editable fields + publish + archive actions.</li>
 *   <li>Published/Superseded/Archived: immutable snapshot (BR-031).</li>
 * </ul>
 * Publishing requires the Publisher role (segregation of duties).
 */
export default function AcknowledgmentVersionDetailPage({ params }: Props) {
  const { definitionId, versionId } = use(params);
  const router = useRouter();

  const { user } = useSession();
  const canAuthor = hasAnyRole(user?.roles ?? [], [
    Roles.AcknowledgmentManager,
    Roles.SystemAdministrator,
  ]);
  const canPublish = hasAnyRole(user?.roles ?? [], [
    Roles.Publisher,
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

  const updateDraft = useUpdateAcknowledgmentVersionDraft(definitionId, versionId);
  const publish = usePublishAcknowledgmentVersion(definitionId, versionId);
  const archiveVersion = useArchiveAcknowledgmentVersion(definitionId, versionId);

  const [publishError, setPublishError] = useState<string | null>(null);
  const [archiveError, setArchiveError] = useState<string | null>(null);
  const [confirmArchive, setConfirmArchive] = useState(false);

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
      <section className="mb-6 flex flex-wrap items-start justify-between gap-4">
        <div>
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
          </div>
          <h1 className="mt-2 flex items-center gap-3 text-3xl font-bold">
            النسخة v{version.versionNumber}
            <AcknowledgmentVersionStatusBadge status={version.status} />
          </h1>
          <p className="mt-2 flex items-center gap-2 text-sm text-[var(--color-text-secondary)]">
            <ActionTypeBadge actionType={version.actionType} />
            {version.versionLabel ? <span>{version.versionLabel}</span> : null}
          </p>
        </div>

        {canAuthor ? (
          <div className="flex flex-wrap items-center gap-2">
            {isDraft && canPublish ? (
              <Button
                onClick={async () => {
                  setPublishError(null);
                  try {
                    await publish.mutateAsync();
                    await refetch();
                  } catch (e) {
                    setPublishError(formatError(e as ApiError));
                  }
                }}
                disabled={publish.isPending}
              >
                {publish.isPending ? "جاري النشر…" : "نشر النسخة"}
              </Button>
            ) : null}

            {isDraft || version.status === AcknowledgmentVersionStatus.Superseded ? (
              confirmArchive ? (
                <>
                  <Button
                    onClick={async () => {
                      setArchiveError(null);
                      try {
                        await archiveVersion.mutateAsync();
                        await refetch();
                        setConfirmArchive(false);
                      } catch (e) {
                        setArchiveError(formatError(e as ApiError));
                      }
                    }}
                    disabled={archiveVersion.isPending}
                  >
                    {archiveVersion.isPending ? "جاري الأرشفة…" : "تأكيد الأرشفة"}
                  </Button>
                  <Button variant="ghost" onClick={() => setConfirmArchive(false)}>
                    تراجع
                  </Button>
                </>
              ) : (
                <Button variant="ghost" onClick={() => setConfirmArchive(true)}>
                  أرشفة النسخة
                </Button>
              )
            ) : null}
          </div>
        ) : null}
      </section>

      {publishError ? (
        <div
          role="alert"
          className="mb-4 rounded-md border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-800"
        >
          {publishError}
        </div>
      ) : null}
      {archiveError ? (
        <div
          role="alert"
          className="mb-4 rounded-md border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-800"
        >
          {archiveError}
        </div>
      ) : null}

      <Card>
        <CardHeader>
          <CardTitle>تفاصيل النسخة</CardTitle>
        </CardHeader>
        <CardBody>
          {canEdit ? (
            <AcknowledgmentVersionForm
              mode="edit"
              initialValue={version}
              defaultActionType={
                definition?.defaultActionType ?? ActionType.SimpleAcknowledgment
              }
              submitLabel="حفظ التعديلات"
              onCancel={() => router.push(`/admin/acknowledgments/${definitionId}`)}
              onSubmit={async (input) => {
                await updateDraft.mutateAsync(
                  input as UpdateAcknowledgmentVersionDraftInput,
                );
              }}
            />
          ) : (
            <VersionSnapshot version={version} />
          )}
        </CardBody>
      </Card>

      <div className="mt-4 grid gap-4 md:grid-cols-2">
        <Card>
          <CardHeader>
            <CardTitle>نموذج التكرار</CardTitle>
            <Link
              href={`/admin/acknowledgments/${definitionId}/versions/${versionId}/recurrence`}
              className="text-sm text-[var(--color-brand-primary)] hover:underline"
            >
              {canEdit ? "تعديل" : "عرض"}
            </Link>
          </CardHeader>
          <CardBody>
            <RecurrenceSummary
              recurrenceModel={version.recurrenceModel}
              startDate={version.startDate}
              dueDate={version.dueDate}
            />
          </CardBody>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>الجمهور المستهدف</CardTitle>
            <Link
              href={`/admin/acknowledgments/${definitionId}/versions/${versionId}/audience`}
              className="text-sm text-[var(--color-brand-primary)] hover:underline"
            >
              {canEdit ? "إدارة" : "عرض"}
            </Link>
          </CardHeader>
          <CardBody>
            <p className="text-sm text-[var(--color-text-secondary)]">
              اضبط قواعد الإدراج والاستثناء وعاين حجم الجمهور قبل النشر
              (BR-032).
            </p>
          </CardBody>
        </Card>
      </div>

      {version.actionType === ActionType.FormBasedDisclosure && (
        <div className="mt-4">
          <Card>
            <CardHeader>
              <CardTitle>تعريف النموذج</CardTitle>
              <Link
                href={`/admin/acknowledgments/${definitionId}/versions/${versionId}/form`}
                className="text-sm text-[var(--color-brand-primary)] hover:underline"
              >
                {canEdit ? "إدارة" : "عرض"}
              </Link>
            </CardHeader>
            <CardBody>
              <p className="text-sm text-[var(--color-text-secondary)]">
                حدد حقول نموذج الإفصاح المطلوبة لهذه النسخة. يجب تعريف النموذج
                قبل النشر (BR-070).
              </p>
            </CardBody>
          </Card>
        </div>
      )}
    </>
  );
}

function VersionSnapshot({
  version,
}: {
  version: NonNullable<ReturnType<typeof useAcknowledgmentVersion>["data"]>;
}) {
  return (
    <dl className="grid gap-4 text-sm md:grid-cols-2">
      <div>
        <dt className="text-xs text-[var(--color-text-tertiary)]">نوع الإجراء</dt>
        <dd>{actionTypeLabel[version.actionType]}</dd>
      </div>
      <div>
        <dt className="text-xs text-[var(--color-text-tertiary)]">
          نسخة السياسة المرتبطة
        </dt>
        <dd dir="ltr">{version.policyVersionId}</dd>
      </div>
      <div>
        <dt className="text-xs text-[var(--color-text-tertiary)]">تاريخ البدء</dt>
        <dd dir="ltr">{version.startDate ?? "—"}</dd>
      </div>
      <div>
        <dt className="text-xs text-[var(--color-text-tertiary)]">تاريخ الاستحقاق</dt>
        <dd dir="ltr">{version.dueDate ?? "—"}</dd>
      </div>
      <div>
        <dt className="text-xs text-[var(--color-text-tertiary)]">تاريخ النشر</dt>
        <dd dir="ltr">
          {version.publishedAtUtc
            ? new Date(version.publishedAtUtc).toLocaleString()
            : "—"}
        </dd>
      </div>
      <div>
        <dt className="text-xs text-[var(--color-text-tertiary)]">نُشرت بواسطة</dt>
        <dd>{version.publishedBy ?? "—"}</dd>
      </div>
      <div>
        <dt className="text-xs text-[var(--color-text-tertiary)]">تاريخ الأرشفة</dt>
        <dd dir="ltr">
          {version.archivedAtUtc
            ? new Date(version.archivedAtUtc).toLocaleString()
            : "—"}
        </dd>
      </div>
      <div>
        <dt className="text-xs text-[var(--color-text-tertiary)]">أُرشفت بواسطة</dt>
        <dd>{version.archivedBy ?? "—"}</dd>
      </div>
      <div className="md:col-span-2">
        <dt className="text-xs text-[var(--color-text-tertiary)]">الملخص</dt>
        <dd className="whitespace-pre-wrap">{version.summary ?? "—"}</dd>
      </div>
      {version.commitmentText ? (
        <div className="md:col-span-2">
          <dt className="text-xs text-[var(--color-text-tertiary)]">نص التعهّد</dt>
          <dd className="whitespace-pre-wrap">{version.commitmentText}</dd>
        </div>
      ) : null}
    </dl>
  );
}

function formatError(err: ApiError): string {
  if (err.errors) {
    return Object.values(err.errors).flat().join(" ") || err.title;
  }
  return err.detail ?? err.title ?? "تعذّر إتمام العملية.";
}
