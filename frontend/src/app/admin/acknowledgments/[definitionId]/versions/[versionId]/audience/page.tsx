"use client";

import Link from "next/link";
import { use, useState } from "react";
import { Button } from "@/components/ui/button";
import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";
import { AudienceExclusionsEditor } from "@/components/audience/AudienceExclusionsEditor";
import { AudiencePreviewSummary } from "@/components/audience/AudiencePreviewSummary";
import { AudienceRulesEditor } from "@/components/audience/AudienceRulesEditor";
import { RecurrenceSummary } from "@/components/recurrence/RecurrenceSummary";
import {
  useAcknowledgmentDefinition,
  useAcknowledgmentVersion,
} from "@/lib/acknowledgments/hooks";
import { AcknowledgmentVersionStatus } from "@/lib/acknowledgments/types";
import {
  useAudience,
  useAudiencePreview,
  useConfigureAudienceExclusions,
  useConfigureAudienceInclusion,
  useSetAllUsersAudience,
} from "@/lib/audience/hooks";
import { audienceTypeLabel } from "@/lib/audience/labels";
import { useSession } from "@/lib/auth/SessionProvider";
import { Roles, hasAnyRole } from "@/lib/auth/roles";

interface Props {
  params: Promise<{ definitionId: string; versionId: string }>;
}

/**
 * Audience-targeting screen (BR-032, BR-050..BR-055). Split into three
 * cards: inclusion rules, exclusion rules, and live preview. Published
 * versions show a read-only summary only.
 */
export default function AudiencePage({ params }: Props) {
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
    data: audience,
    isLoading: audienceLoading,
    refetch: refetchAudience,
  } = useAudience(definitionId, versionId);

  const preview = useAudiencePreview(definitionId, versionId, {
    enabled: !!audience,
  });

  const configureInclusion = useConfigureAudienceInclusion(definitionId, versionId);
  const configureExclusions = useConfigureAudienceExclusions(definitionId, versionId);
  const setAllUsers = useSetAllUsersAudience(definitionId, versionId);

  const [banner, setBanner] = useState<string | null>(null);

  if (versionLoading) {
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
        <h1 className="mt-2 text-3xl font-bold">الجمهور المستهدف</h1>
        <p className="mt-2 text-sm text-[var(--color-text-secondary)]">
          حدد من يجب أن يُوقّع هذه النسخة. القواعد داخل كل قائمة تُجمَع بـ OR،
          والاستثناءات تُقدَّم دائماً على الإدراج (BR-055).
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

      <Card className="mb-4">
        <CardHeader>
          <CardTitle>ملخص التكرار</CardTitle>
          <Link
            className="text-sm text-[var(--color-brand-primary)] hover:underline"
            href={`/admin/acknowledgments/${definitionId}/versions/${versionId}/recurrence`}
          >
            تعديل نموذج التكرار
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

      <Card className="mb-4">
        <CardHeader>
          <CardTitle>قواعد الإدراج</CardTitle>
          {audience ? (
            <span className="text-xs text-[var(--color-text-tertiary)]">
              نوع الجمهور الحالي: {audienceTypeLabel(audience.audienceType)}
            </span>
          ) : null}
        </CardHeader>
        <CardBody>
          {audienceLoading ? (
            <p className="text-sm text-[var(--color-text-tertiary)]">
              جاري تحميل الجمهور…
            </p>
          ) : canEdit ? (
            <AudienceRulesEditor
              initialRules={audience?.inclusionRules ?? null}
              onSave={async (rules) => {
                await configureInclusion.mutateAsync({ rules });
                await refetchAudience();
                setBanner("تم حفظ قواعد الإدراج.");
              }}
              onSetAllUsers={async () => {
                await setAllUsers.mutateAsync();
                await refetchAudience();
                setBanner("تم تعيين الجمهور إلى كل المستخدمين.");
              }}
            />
          ) : (
            <ReadOnlyRules
              rules={audience?.inclusionRules ?? []}
              emptyLabel="لم يتم ضبط قواعد إدراج."
            />
          )}
        </CardBody>
      </Card>

      <Card className="mb-4">
        <CardHeader>
          <CardTitle>قواعد الاستثناء</CardTitle>
        </CardHeader>
        <CardBody>
          {canEdit ? (
            <AudienceExclusionsEditor
              initialRules={audience?.exclusionRules ?? []}
              onSave={async (rules) => {
                await configureExclusions.mutateAsync({ rules });
                await refetchAudience();
                setBanner("تم حفظ الاستثناءات.");
              }}
            />
          ) : (
            <ReadOnlyRules
              rules={audience?.exclusionRules ?? []}
              emptyLabel="لا توجد استثناءات مضبوطة."
            />
          )}
        </CardBody>
      </Card>

      <Card>
        <CardBody>
          <AudiencePreviewSummary
            preview={preview.data}
            isLoading={preview.isLoading}
            isError={preview.isError}
            errorMessage={preview.error?.title ?? preview.error?.detail ?? null}
            onRefresh={() => preview.refetch()}
          />
        </CardBody>
      </Card>
    </>
  );
}

function ReadOnlyRules({
  rules,
  emptyLabel,
}: {
  rules: { id: string; ruleType: number; ruleValue: string | null }[];
  emptyLabel: string;
}) {
  if (rules.length === 0) {
    return <p className="text-sm text-[var(--color-text-tertiary)]">{emptyLabel}</p>;
  }
  return (
    <ul className="divide-y divide-[var(--color-border-default)] rounded-[10px] border border-[var(--color-border-default)] bg-white">
      {rules.map((rule) => (
        <li
          key={rule.id}
          className="flex items-center justify-between px-3 py-2 text-sm"
        >
          <span>{ruleTypeLabel(rule.ruleType)}</span>
          <span dir="ltr" className="text-[var(--color-text-tertiary)]">
            {rule.ruleValue ?? "—"}
          </span>
        </li>
      ))}
    </ul>
  );
}

function ruleTypeLabel(type: number): string {
  switch (type) {
    case 0:
      return "كل المستخدمين";
    case 1:
      return "قسم";
    case 2:
      return "مجموعة AD";
    case 3:
      return "مستخدم محدد";
    default:
      return "غير معروف";
  }
}
