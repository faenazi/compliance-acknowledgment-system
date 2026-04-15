"use client";

import Link from "next/link";
import { use, useState } from "react";
import { useRouter } from "next/navigation";
import { Button } from "@/components/ui/button";
import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";
import { PolicyVersionStatusBadge } from "@/components/policies/PolicyStatusBadge";
import { PolicyDocumentUpload } from "@/components/policies/PolicyDocumentUpload";
import { VersionForm } from "@/components/policies/VersionForm";
import {
  useArchivePolicyVersion,
  usePolicy,
  usePolicyVersion,
  usePublishPolicyVersion,
  useUpdatePolicyVersionDraft,
} from "@/lib/policies/hooks";
import { PolicyVersionStatus, type UpdatePolicyVersionDraftInput } from "@/lib/policies/types";
import { useSession } from "@/lib/auth/SessionProvider";
import { Roles, hasAnyRole } from "@/lib/auth/roles";
import type { ApiError } from "@/lib/api/client";

interface VersionPageProps {
  params: Promise<{ policyId: string; versionId: string }>;
}

/**
 * Policy version detail page.
 * <ul>
 *   <li>Draft: editable fields, document upload, publish + archive actions.</li>
 *   <li>Published/Superseded/Archived: read-only snapshot (BR-003).</li>
 * </ul>
 */
export default function PolicyVersionDetailPage({ params }: VersionPageProps) {
  const { policyId, versionId } = use(params);
  const router = useRouter();

  const { user } = useSession();
  const canAuthor = hasAnyRole(user?.roles ?? [], [Roles.PolicyManager, Roles.SystemAdministrator]);
  const canPublish = hasAnyRole(user?.roles ?? [], [Roles.Publisher, Roles.SystemAdministrator]);

  const { data: policy } = usePolicy(policyId);
  const { data: version, isLoading, isError, error, refetch } = usePolicyVersion(policyId, versionId);
  const updateDraft = useUpdatePolicyVersionDraft(policyId, versionId);
  const publish = usePublishPolicyVersion(policyId, versionId);
  const archiveVersion = useArchivePolicyVersion(policyId, versionId);

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
          <p className="text-sm text-red-700">تعذّر تحميل النسخة: {error?.title ?? "غير متاح"}</p>
          <div className="mt-3">
            <Button variant="secondary" onClick={() => refetch()}>إعادة المحاولة</Button>
          </div>
        </CardBody>
      </Card>
    );
  }

  const isDraft = version.status === PolicyVersionStatus.Draft;
  const canEdit = canAuthor && isDraft;

  return (
    <>
      <section className="mb-6 flex flex-wrap items-start justify-between gap-4">
        <div>
          <div className="text-xs text-[var(--color-text-tertiary)]">
            <Link href="/admin/policies" className="hover:underline">السياسات</Link>
            <span className="mx-2">›</span>
            <Link href={`/admin/policies/${policyId}`} className="hover:underline">
              {policy?.title ?? "تفاصيل السياسة"}
            </Link>
            <span className="mx-2">›</span>
            <Link href={`/admin/policies/${policyId}/versions`} className="hover:underline">النسخ</Link>
          </div>
          <h1 className="mt-2 flex items-center gap-3 text-3xl font-bold">
            النسخة v{version.versionNumber}
            <PolicyVersionStatusBadge status={version.status} />
          </h1>
          {version.versionLabel ? (
            <p className="mt-2 text-sm text-[var(--color-text-secondary)]">{version.versionLabel}</p>
          ) : null}
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
                disabled={publish.isPending || !version.document}
              >
                {publish.isPending ? "جاري النشر…" : "نشر النسخة"}
              </Button>
            ) : null}

            {isDraft || version.status === PolicyVersionStatus.Superseded ? (
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

      <div className="grid gap-6 lg:grid-cols-5">
        <div className="lg:col-span-3">
          <Card>
            <CardHeader>
              <CardTitle>تفاصيل النسخة</CardTitle>
            </CardHeader>
            <CardBody>
              {canEdit ? (
                <VersionForm
                  mode="edit"
                  initialValue={version}
                  submitLabel="حفظ التعديلات"
                  onCancel={() => router.push(`/admin/policies/${policyId}`)}
                  onSubmit={async (input) => {
                    await updateDraft.mutateAsync(input as UpdatePolicyVersionDraftInput);
                  }}
                />
              ) : (
                <VersionSnapshot
                  effectiveDate={version.effectiveDate}
                  summary={version.summary}
                  publishedAtUtc={version.publishedAtUtc}
                  publishedBy={version.publishedBy}
                  archivedAtUtc={version.archivedAtUtc}
                  archivedBy={version.archivedBy}
                />
              )}
            </CardBody>
          </Card>
        </div>

        <div className="lg:col-span-2">
          <Card>
            <CardHeader>
              <CardTitle>الملف المرفق</CardTitle>
            </CardHeader>
            <CardBody>
              <PolicyDocumentUpload
                policyId={policyId}
                versionId={versionId}
                existingDocument={version.document}
                disabled={!canEdit}
              />
            </CardBody>
          </Card>
        </div>
      </div>
    </>
  );
}

function VersionSnapshot({
  effectiveDate,
  summary,
  publishedAtUtc,
  publishedBy,
  archivedAtUtc,
  archivedBy,
}: {
  effectiveDate: string | null;
  summary: string | null;
  publishedAtUtc: string | null;
  publishedBy: string | null;
  archivedAtUtc: string | null;
  archivedBy: string | null;
}) {
  return (
    <dl className="grid gap-4 text-sm md:grid-cols-2">
      <div>
        <dt className="text-xs text-[var(--color-text-tertiary)]">تاريخ النفاذ</dt>
        <dd dir="ltr">{effectiveDate ?? "—"}</dd>
      </div>
      <div>
        <dt className="text-xs text-[var(--color-text-tertiary)]">تاريخ النشر</dt>
        <dd dir="ltr">{publishedAtUtc ? new Date(publishedAtUtc).toLocaleString() : "—"}</dd>
      </div>
      <div>
        <dt className="text-xs text-[var(--color-text-tertiary)]">نُشرت بواسطة</dt>
        <dd>{publishedBy ?? "—"}</dd>
      </div>
      <div>
        <dt className="text-xs text-[var(--color-text-tertiary)]">تاريخ الأرشفة</dt>
        <dd dir="ltr">{archivedAtUtc ? new Date(archivedAtUtc).toLocaleString() : "—"}</dd>
      </div>
      <div>
        <dt className="text-xs text-[var(--color-text-tertiary)]">أُرشفت بواسطة</dt>
        <dd>{archivedBy ?? "—"}</dd>
      </div>
      <div className="md:col-span-2">
        <dt className="text-xs text-[var(--color-text-tertiary)]">ملخص التغييرات</dt>
        <dd className="whitespace-pre-wrap">{summary ?? "—"}</dd>
      </div>
    </dl>
  );
}

function formatError(err: ApiError): string {
  if (err.errors) {
    return Object.values(err.errors).flat().join(" ") || err.title;
  }
  return err.detail ?? err.title ?? "تعذّر إتمام العملية.";
}
