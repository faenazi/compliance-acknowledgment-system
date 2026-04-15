"use client";

import Link from "next/link";
import { use, useState } from "react";
import { useRouter } from "next/navigation";
import { Button } from "@/components/ui/button";
import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";
import { AcknowledgmentDefinitionForm } from "@/components/acknowledgments/AcknowledgmentDefinitionForm";
import {
  AcknowledgmentStatusBadge,
  AcknowledgmentVersionStatusBadge,
} from "@/components/acknowledgments/AcknowledgmentStatusBadge";
import { ActionTypeBadge } from "@/components/acknowledgments/ActionTypeBadge";
import {
  useAcknowledgmentDefinition,
  useArchiveAcknowledgmentDefinition,
  useUpdateAcknowledgmentDefinition,
} from "@/lib/acknowledgments/hooks";
import {
  AcknowledgmentStatus,
  type AcknowledgmentVersionSummary,
  type UpdateAcknowledgmentDefinitionInput,
} from "@/lib/acknowledgments/types";
import { useSession } from "@/lib/auth/SessionProvider";
import { Roles, hasAnyRole } from "@/lib/auth/roles";

interface Props {
  params: Promise<{ definitionId: string }>;
}

/**
 * Acknowledgment definition detail / edit page. Combines the mutable master
 * form with the version history. Version-level transitions (publish/archive)
 * live on the version detail page — this screen is the hub.
 */
export default function AcknowledgmentDefinitionDetailPage({ params }: Props) {
  const { definitionId } = use(params);
  const router = useRouter();

  const { user } = useSession();
  const canAuthor = hasAnyRole(user?.roles ?? [], [
    Roles.AcknowledgmentManager,
    Roles.SystemAdministrator,
  ]);

  const { data: definition, isLoading, isError, error, refetch } =
    useAcknowledgmentDefinition(definitionId);
  const update = useUpdateAcknowledgmentDefinition(definitionId);
  const archive = useArchiveAcknowledgmentDefinition(definitionId);
  const [confirmArchive, setConfirmArchive] = useState(false);

  if (isLoading) {
    return <p className="text-sm text-[var(--color-text-tertiary)]">جاري التحميل…</p>;
  }
  if (isError || !definition) {
    return (
      <Card>
        <CardBody>
          <p className="text-sm text-red-700">
            تعذّر تحميل الإقرار: {error?.title ?? "غير متاح"}
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

  const isArchived = definition.status === AcknowledgmentStatus.Archived;

  return (
    <>
      <section className="mb-6 flex flex-wrap items-start justify-between gap-4">
        <div>
          <div className="text-xs text-[var(--color-text-tertiary)]">
            <Link href="/admin/acknowledgments" className="hover:underline">
              الإقرارات
            </Link>
          </div>
          <h1 className="mt-2 flex items-center gap-3 text-3xl font-bold">
            {definition.title}
            <AcknowledgmentStatusBadge status={definition.status} />
          </h1>
          <p className="mt-2 flex items-center gap-2 text-sm text-[var(--color-text-secondary)]">
            <span>{definition.ownerDepartment}</span>
            <span>·</span>
            <ActionTypeBadge actionType={definition.defaultActionType} />
          </p>
        </div>

        {canAuthor && !isArchived ? (
          <div className="flex items-center gap-2">
            {confirmArchive ? (
              <>
                <Button
                  onClick={async () => {
                    await archive.mutateAsync();
                    setConfirmArchive(false);
                  }}
                  disabled={archive.isPending}
                >
                  {archive.isPending ? "جاري الأرشفة…" : "تأكيد الأرشفة"}
                </Button>
                <Button variant="ghost" onClick={() => setConfirmArchive(false)}>
                  تراجع
                </Button>
              </>
            ) : (
              <Button variant="ghost" onClick={() => setConfirmArchive(true)}>
                أرشفة الإقرار
              </Button>
            )}
          </div>
        ) : null}
      </section>

      <div className="grid gap-6 lg:grid-cols-5">
        <div className="lg:col-span-3">
          <Card>
            <CardHeader>
              <CardTitle>البيانات الأساسية</CardTitle>
            </CardHeader>
            <CardBody>
              <AcknowledgmentDefinitionForm
                mode="edit"
                initialValue={definition}
                submitLabel="حفظ التعديلات"
                onSubmit={async (input) => {
                  if (!canAuthor) return;
                  await update.mutateAsync(input as UpdateAcknowledgmentDefinitionInput);
                }}
              />
            </CardBody>
          </Card>
        </div>

        <div className="lg:col-span-2">
          <Card>
            <CardHeader>
              <CardTitle>النسخ</CardTitle>
              {canAuthor && !isArchived ? (
                <Link
                  href={`/admin/acknowledgments/${definition.id}/versions/new`}
                >
                  <Button size="sm" variant="secondary">
                    + نسخة جديدة
                  </Button>
                </Link>
              ) : null}
            </CardHeader>
            <CardBody>
              {definition.versions.length === 0 ? (
                <p className="text-sm text-[var(--color-text-tertiary)]">
                  لا توجد نسخ مُنشأة لهذا الإقرار بعد.
                </p>
              ) : (
                <ul className="divide-y divide-[var(--color-border-default)]">
                  {definition.versions.map((v) => (
                    <VersionRow
                      key={v.id}
                      definitionId={definition.id}
                      version={v}
                      isCurrent={definition.currentAcknowledgmentVersionId === v.id}
                      onOpen={() =>
                        router.push(
                          `/admin/acknowledgments/${definition.id}/versions/${v.id}`,
                        )
                      }
                    />
                  ))}
                </ul>
              )}
            </CardBody>
          </Card>
        </div>
      </div>
    </>
  );
}

function VersionRow({
  version,
  isCurrent,
  onOpen,
}: {
  definitionId: string;
  version: AcknowledgmentVersionSummary;
  isCurrent: boolean;
  onOpen: () => void;
}) {
  return (
    <li className="flex items-center justify-between py-3 text-sm">
      <div>
        <button
          type="button"
          onClick={onOpen}
          className="font-medium text-[var(--color-text-link)] hover:underline"
        >
          v{version.versionNumber}
          {version.versionLabel ? ` — ${version.versionLabel}` : ""}
        </button>
        <div className="mt-1 flex items-center gap-2 text-xs text-[var(--color-text-tertiary)]">
          <AcknowledgmentVersionStatusBadge status={version.status} />
          <ActionTypeBadge actionType={version.actionType} />
          {isCurrent ? (
            <span className="text-[var(--color-brand-primary)]">الحالية</span>
          ) : null}
          {version.dueDate ? <span dir="ltr">· حتى {version.dueDate}</span> : null}
        </div>
      </div>
      <Button variant="ghost" size="sm" onClick={onOpen}>
        تفاصيل
      </Button>
    </li>
  );
}
