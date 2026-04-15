"use client";

import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { useSession } from "@/lib/auth/SessionProvider";

/**
 * Admin portal landing page. Sprint 1 establishes the authenticated
 * shell and role gate; operational cards (policies, acknowledgments,
 * compliance) land progressively from Sprint 2 onwards.
 */
export default function AdminDashboardPage() {
  const { user } = useSession();

  return (
    <>
      <section className="mb-6">
        <h1 className="text-3xl font-bold">لوحة الإدارة</h1>
        <p className="mt-2 text-[var(--color-text-secondary)]">
          {user
            ? `مرحباً ${user.displayName} — الوحدات التشغيلية ستتاح تباعاً.`
            : "سيتم عرض ملخصات السياسات والإقرارات والامتثال هنا."}
        </p>
      </section>

      <div className="grid gap-6 md:grid-cols-3">
        <Card>
          <CardHeader>
            <CardTitle>السياسات</CardTitle>
            <Badge status="draft">قيد الإعداد</Badge>
          </CardHeader>
          <CardBody>
            <p>سيتم تفعيل إدارة السياسات في Sprint 2.</p>
          </CardBody>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>الإقرارات</CardTitle>
            <Badge status="draft">قيد الإعداد</Badge>
          </CardHeader>
          <CardBody>
            <p>سيتم تفعيل إدارة الإقرارات في Sprint 3.</p>
          </CardBody>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>الامتثال</CardTitle>
            <Badge status="pending">—</Badge>
          </CardHeader>
          <CardBody>
            <p>سيتم تفعيل مؤشرات الامتثال في Sprint 8.</p>
          </CardBody>
        </Card>
      </div>
    </>
  );
}
