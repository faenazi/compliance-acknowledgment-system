"use client";

import { useRouter } from "next/navigation";
import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";
import { AcknowledgmentDefinitionForm } from "@/components/acknowledgments/AcknowledgmentDefinitionForm";
import { useCreateAcknowledgmentDefinition } from "@/lib/acknowledgments/hooks";
import { useSession } from "@/lib/auth/SessionProvider";
import { Roles, hasAnyRole } from "@/lib/auth/roles";
import type { CreateAcknowledgmentDefinitionInput } from "@/lib/acknowledgments/types";

export default function NewAcknowledgmentPage() {
  const router = useRouter();
  const { user } = useSession();
  const canAuthor = hasAnyRole(user?.roles ?? [], [
    Roles.AcknowledgmentManager,
    Roles.SystemAdministrator,
  ]);

  const create = useCreateAcknowledgmentDefinition();

  if (!canAuthor) {
    return (
      <Card>
        <CardBody>
          <p className="text-sm text-[var(--color-text-tertiary)]">
            لا تملك صلاحية إنشاء تعريفات إقرار جديدة.
          </p>
        </CardBody>
      </Card>
    );
  }

  return (
    <>
      <section className="mb-6">
        <h1 className="text-3xl font-bold">إقرار جديد</h1>
        <p className="mt-2 text-[var(--color-text-secondary)]">
          أدخل البيانات الأساسية للإقرار. يمكن إضافة نسخة مرتبطة بسياسة منشورة
          لاحقاً من صفحة التفاصيل.
        </p>
      </section>

      <Card>
        <CardHeader>
          <CardTitle>بيانات الإقرار</CardTitle>
        </CardHeader>
        <CardBody>
          <AcknowledgmentDefinitionForm
            mode="create"
            submitLabel="إنشاء"
            onCancel={() => router.push("/admin/acknowledgments")}
            onSubmit={async (input) => {
              const created = await create.mutateAsync(
                input as CreateAcknowledgmentDefinitionInput,
              );
              router.push(`/admin/acknowledgments/${created.id}`);
              return created;
            }}
          />
        </CardBody>
      </Card>
    </>
  );
}
