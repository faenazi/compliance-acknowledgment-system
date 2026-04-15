"use client";

import Link from "next/link";
import { use } from "react";
import { useRouter } from "next/navigation";
import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";
import { VersionForm } from "@/components/policies/VersionForm";
import { useCreatePolicyVersion, usePolicy } from "@/lib/policies/hooks";
import { useSession } from "@/lib/auth/SessionProvider";
import { Roles, hasAnyRole } from "@/lib/auth/roles";
import type { CreatePolicyVersionInput } from "@/lib/policies/types";

interface NewVersionPageProps {
  params: Promise<{ policyId: string }>;
}

export default function NewPolicyVersionPage({ params }: NewVersionPageProps) {
  const { policyId } = use(params);
  const router = useRouter();
  const { user } = useSession();
  const canAuthor = hasAnyRole(user?.roles ?? [], [Roles.PolicyManager, Roles.SystemAdministrator]);

  const { data: policy } = usePolicy(policyId);
  const create = useCreatePolicyVersion(policyId);

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
          <Link href="/admin/policies" className="hover:underline">السياسات</Link>
          <span className="mx-2">›</span>
          <Link href={`/admin/policies/${policyId}`} className="hover:underline">
            {policy?.title ?? "تفاصيل السياسة"}
          </Link>
        </div>
        <h1 className="mt-2 text-3xl font-bold">نسخة جديدة</h1>
        <p className="mt-2 text-[var(--color-text-secondary)]">
          يتم إنشاء النسخة كمسودة. يمكن تعديل محتواها ورفع الملف قبل النشر.
        </p>
      </section>

      <Card>
        <CardHeader>
          <CardTitle>بيانات النسخة</CardTitle>
        </CardHeader>
        <CardBody>
          <VersionForm
            mode="create"
            submitLabel="إنشاء النسخة"
            onCancel={() => router.push(`/admin/policies/${policyId}`)}
            onSubmit={async (input) => {
              const created = await create.mutateAsync(input as CreatePolicyVersionInput);
              router.push(`/admin/policies/${policyId}/versions/${created.id}`);
              return created;
            }}
          />
        </CardBody>
      </Card>
    </>
  );
}
