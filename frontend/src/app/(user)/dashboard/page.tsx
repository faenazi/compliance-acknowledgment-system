"use client";

import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { useSession } from "@/lib/auth/SessionProvider";

/**
 * Employee dashboard shell. Sprint 1 surfaces the authenticated user's
 * identity only; the full required/pending/overdue/completed breakdown
 * arrives with Sprint 6 (User Portal).
 */
export default function UserDashboardPage() {
  const { user } = useSession();

  return (
    <>
      <section className="mb-6">
        <h1 className="text-3xl font-bold">
          {user ? `مرحباً، ${user.displayName}` : "لوحة الموظف"}
        </h1>
        <p className="mt-2 text-[var(--color-text-secondary)]">
          سيتم عرض الإقرارات المطلوبة والمتأخرة والمكتملة هنا.
        </p>
      </section>

      <div className="grid gap-6 md:grid-cols-3">
        <Card>
          <CardHeader>
            <CardTitle>المعلّقة</CardTitle>
            <Badge status="pending">0</Badge>
          </CardHeader>
          <CardBody>
            <p>سيتم تفعيل قائمة الإجراءات المطلوبة في Sprint 6.</p>
          </CardBody>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>المتأخرة</CardTitle>
            <Badge status="overdue">0</Badge>
          </CardHeader>
          <CardBody>
            <p>سيتم تفعيل قائمة الإجراءات المتأخرة في Sprint 6.</p>
          </CardBody>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>المكتملة</CardTitle>
            <Badge status="completed">0</Badge>
          </CardHeader>
          <CardBody>
            <p>سيتم تفعيل السجل الشخصي في Sprint 6.</p>
          </CardBody>
        </Card>
      </div>
    </>
  );
}
