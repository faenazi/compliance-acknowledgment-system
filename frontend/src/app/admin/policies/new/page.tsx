"use client";

import { useRouter } from "next/navigation";
import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";
import { PolicyForm } from "@/components/policies/PolicyForm";
import { useCreatePolicy } from "@/lib/policies/hooks";
import { useSession } from "@/lib/auth/SessionProvider";
import { Roles, hasAnyRole } from "@/lib/auth/roles";
import type { CreatePolicyInput } from "@/lib/policies/types";

export default function NewPolicyPage() {
  const router = useRouter();
  const { user } = useSession();
  const canAuthor = hasAnyRole(user?.roles ?? [], [Roles.PolicyManager, Roles.SystemAdministrator]);

  const create = useCreatePolicy();

  if (!canAuthor) {
    return (
      <Card>
        <CardBody>
          <p className="text-sm text-[var(--color-text-tertiary)]">
            لا تملك صلاحية إنشاء سياسات جديدة.
          </p>
        </CardBody>
      </Card>
    );
  }

  return (
    <>
      <section className="mb-6">
        <h1 className="text-3xl font-bold">سياسة جديدة</h1>
        <p className="mt-2 text-[var(--color-text-secondary)]">
          أدخل البيانات الأساسية للسياسة. يمكن إضافة نسخة ومستند لاحقاً من صفحة التفاصيل.
        </p>
      </section>

      <Card>
        <CardHeader>
          <CardTitle>بيانات السياسة</CardTitle>
        </CardHeader>
        <CardBody>
          <PolicyForm
            mode="create"
            submitLabel="إنشاء"
            onCancel={() => router.push("/admin/policies")}
            onSubmit={async (input) => {
              const created = await create.mutateAsync(input as CreatePolicyInput);
              router.push(`/admin/policies/${created.id}`);
              return created;
            }}
          />
        </CardBody>
      </Card>
    </>
  );
}