"use client";

import Link from "next/link";
import { use, useState } from "react";
import { useRouter } from "next/navigation";
import { Button } from "@/components/ui/button";
import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";
import { PolicyForm } from "@/components/policies/PolicyForm";
import { PolicyStatusBadge, PolicyVersionStatusBadge } from "@/components/policies/PolicyStatusBadge";
import { useArchivePolicy, usePolicy, useUpdatePolicy } from "@/lib/policies/hooks";
import { PolicyStatus, type PolicyVersionSummary } from "@/lib/policies/types";
import { useSession } from "@/lib/auth/SessionProvider";
import { Roles, hasAnyRole } from "@/lib/auth/roles";
import type { UpdatePolicyInput } from "@/lib/policies/types";

interface PolicyPageProps {
  params: Promise<{ policyId: string }>;
}

/**
 * Policy detail / edit page. Combines the mutable master form with the
 * version history table. Version-level transitions live on the version
 * detail page — this screen is the hub.
 */
export default function PolicyDetailPage({ params }: PolicyPageProps) {
  const { policyId } = use(params);
  const router = useRouter();

  const { user } = useSession();
  const canAuthor = hasAnyRole(user?.roles ?? [], [Roles.PolicyManager, Roles.SystemAdministrator]);

  const { data: policy, isLoading, isError, error, refetch } = usePolicy(policyId);
  const update = useUpdatePolicy(policyId);
  const archive = useArchivePolicy(policyId);
  const [confirmArchive, setConfirmArchive] = useState(false);

  if (isLoading) {
    return <p className="text-sm text-[var(--color-text-tertiary)]">جاري التحميل…</p>;
  }
  if (isError || !policy) {
    return (
      <Card>
        <CardBody>
          <p className="text-sm text-red-700">تعذّر تحميل السياسة: {error?.title ?? "غير متاح"}</p>
          <div className="mt-3">
            <Button variant="secondary" onClick={() => refetch()}>إعادة المحاولة</Button>
          </div>
        </CardBody>
      </Card>
    );
  }

  const isArchived = policy.status === PolicyStatus.Archived;

  return (
    <>
      <section className="mb-6 flex flex-wrap items-start justify-between gap-4">
        <div>
          <div className="text-xs text-[var(--color-text-tertiary)]">
            <Link href="/admin/policies" className="hover:underline">السياسات</Link>
            <span className="mx-2">›</span>
            <span dir="ltr" className="font-mono">{policy.policyCode}</span>
          </div>
          <h1 className="mt-2 flex items-center gap-3 text-3xl font-bold">
            {policy.title}
            <PolicyStatusBadge status={policy.status} />
          </h1>
          <p className="mt-2 text-sm text-[var(--color-text-secondary)]">
            {policy.ownerDepartment}
            {policy.category ? ` · ${policy.category}` : ""}
          </p>
        </div>

        {canAuthor && !isArchived ? (
          <div className="flex items-center gap-2">
            <Link href={`/admin/policies/${policy.id}/versions`}>
              <Button variant="secondary">إدارة النسخ</Button>
            </Link>
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
                أرشفة السياسة
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
              <PolicyForm
                mode="edit"
                initialValue={policy}
                submitLabel="حفظ التعديلات"
                onSubmit={async (input) => {
                  if (!canAuthor) return;
                  await update.mutateAsync(input as UpdatePolicyInput);
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
                <Link href={`/admin/policies/${policy.id}/versions/new`}>
                  <Button size="sm" variant="secondary">+ نسخة جديدة</Button>
                </Link>
              ) : null}
            </CardHeader>
            <CardBody>
              {policy.versions.length === 0 ? (
                <p className="text-sm text-[var(--color-text-tertiary)]">
                  لا توجد نسخ مُنشأة لهذه السياسة بعد.
                </p>
              ) : (
                <ul className="divide-y divide-[var(--color-border-default)]">
                  {policy.versions.map((v) => (
                    <VersionRow
                      key={v.id}
                      policyId={policy.id}
                      version={v}
                      isCurrent={policy.currentPolicyVersionId === v.id}
                      onOpen={() => router.push(`/admin/policies/${policy.id}/versions/${v.id}`)}
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
  policyId: string;
  version: PolicyVersionSummary;
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
          <PolicyVersionStatusBadge status={version.status} />
          {isCurrent ? <span className="text-[var(--color-brand-primary)]">الحالية</span> : null}
          {version.effectiveDate ? <span dir="ltr">{version.effectiveDate}</span> : null}
          {!version.hasDocument ? <span>· بدون ملف</span> : null}
        </div>
      </div>
      <Button variant="ghost" size="sm" onClick={onOpen}>
        تفاصيل
      </Button>
    </li>
  );
}
