"use client";

import Link from "next/link";
import { use } from "react";
import { useRouter } from "next/navigation";
import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";
import { AcknowledgmentVersionForm } from "@/components/acknowledgments/AcknowledgmentVersionForm";
import {
  useAcknowledgmentDefinition,
  useCreateAcknowledgmentVersion,
} from "@/lib/acknowledgments/hooks";
import { useSession } from "@/lib/auth/SessionProvider";
import { Roles, hasAnyRole } from "@/lib/auth/roles";
import {
  ActionType,
  type CreateAcknowledgmentVersionInput,
} from "@/lib/acknowledgments/types";

interface Props {
  params: Promise<{ definitionId: string }>;
}

export default function NewAcknowledgmentVersionPage({ params }: Props) {
  const { definitionId } = use(params);
  const router = useRouter();
  const { user } = useSession();
  const canAuthor = hasAnyRole(user?.roles ?? [], [
    Roles.AcknowledgmentManager,
    Roles.SystemAdministrator,
  ]);

  const { data: definition } = useAcknowledgmentDefinition(definitionId);
  const create = useCreateAcknowledgmentVersion(definitionId);

  if (!canAuthor) {
    return (
      <Card>
        <CardBody>
          <p className="text-sm text-[var(--color-text-tertiary)]">
            لا تملك صلاحية إنشاء نسخ جديدة.
          </p>
        </CardBody>
      </Card>
    );
  }

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
        </div>
        <h1 className="mt-2 text-3xl font-bold">نسخة إقرار جديدة</h1>
        <p className="mt-2 text-[var(--color-text-secondary)]">
          يتم إنشاء النسخة كمسودة مرتبطة بنسخة منشورة من السياسة. يمكن تعديلها
          قبل النشر.
        </p>
      </section>

      <Card>
        <CardHeader>
          <CardTitle>بيانات النسخة</CardTitle>
        </CardHeader>
        <CardBody>
          <AcknowledgmentVersionForm
            mode="create"
            defaultActionType={
              definition?.defaultActionType ?? ActionType.SimpleAcknowledgment
            }
            submitLabel="إنشاء النسخة"
            onCancel={() => router.push(`/admin/acknowledgments/${definitionId}`)}
            onSubmit={async (input) => {
              const created = await create.mutateAsync(
                input as CreateAcknowledgmentVersionInput,
              );
              router.push(
                `/admin/acknowledgments/${definitionId}/versions/${created.id}`,
              );
              return created;
            }}
          />
        </CardBody>
      </Card>
    </>
  );
}
