import { AppShell } from "@/components/layout/AppShell";
import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";

/**
 * Admin portal dashboard — placeholder for Sprint 0.
 * Operational cards (policies, acknowledgments, compliance) land from Sprint 7.
 */
export default function AdminDashboardPage() {
  return (
    <AppShell>
      <section className="mb-6">
        <h1 className="text-3xl font-bold">لوحة الإدارة</h1>
        <p className="mt-2 text-[var(--color-text-secondary)]">
          سيتم عرض ملخصات السياسات والإقرارات والامتثال هنا.
        </p>
      </section>

      <div className="grid gap-6 md:grid-cols-3">
        <Card>
          <CardHeader>
            <CardTitle>السياسات</CardTitle>
            <Badge status="draft">0</Badge>
          </CardHeader>
          <CardBody>
            <p>قيد الإعداد.</p>
          </CardBody>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>الإقرارات</CardTitle>
            <Badge status="published">0</Badge>
          </CardHeader>
          <CardBody>
            <p>قيد الإعداد.</p>
          </CardBody>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>الامتثال</CardTitle>
            <Badge status="pending">0%</Badge>
          </CardHeader>
          <CardBody>
            <p>قيد الإعداد.</p>
          </CardBody>
        </Card>
      </div>
    </AppShell>
  );
}
