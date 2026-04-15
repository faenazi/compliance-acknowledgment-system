import Link from "next/link";
import { AppShell } from "@/components/layout/AppShell";
import { Button } from "@/components/ui/button";
import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";

/**
 * Sprint 0 landing page.
 * Verifies that the app shell, providers, tokens, and routing foundation work.
 * Replaced in Sprint 1 by an authenticated redirect to the role-appropriate dashboard.
 */
export default function HomePage() {
  return (
    <AppShell>
      <section className="mb-10">
        <h1 className="text-3xl font-bold leading-tight">
          منصة الإقرارات المؤسسية
        </h1>
        <p className="mt-2 text-[var(--color-text-secondary)]">
          الأساس التقني جاهز — سيتم تفعيل المصادقة والوحدات تباعاً.
        </p>
      </section>

      <div className="grid gap-6 md:grid-cols-2">
        <Card>
          <CardHeader>
            <CardTitle>بوابة الموظف</CardTitle>
            <Badge status="pending">قيد الإعداد</Badge>
          </CardHeader>
          <CardBody>
            <p>لوحة المستخدم والإقرارات المطلوبة والسجل الشخصي.</p>
            <Link href="/dashboard" className="inline-block">
              <Button variant="secondary">الانتقال إلى لوحة الموظف</Button>
            </Link>
          </CardBody>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>بوابة الإدارة</CardTitle>
            <Badge status="draft">قيد الإعداد</Badge>
          </CardHeader>
          <CardBody>
            <p>إدارة السياسات والإقرارات والجمهور والتقارير.</p>
            <Link href="/admin/dashboard" className="inline-block">
              <Button variant="secondary">الانتقال إلى لوحة الإدارة</Button>
            </Link>
          </CardBody>
        </Card>
      </div>
    </AppShell>
  );
}
