import { AppShell } from "@/components/layout/AppShell";
import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";

/**
 * User portal dashboard — placeholder for Sprint 0.
 * Full content (required actions, overdue, history summary) lands in Sprint 6.
 */
export default function UserDashboardPage() {
  return (
    <AppShell>
      <section className="mb-6">
        <h1 className="text-3xl font-bold">لوحة الموظف</h1>
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
            <p>لا توجد بيانات في مرحلة Sprint 0.</p>
          </CardBody>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>المتأخرة</CardTitle>
            <Badge status="overdue">0</Badge>
          </CardHeader>
          <CardBody>
            <p>لا توجد بيانات في مرحلة Sprint 0.</p>
          </CardBody>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>المكتملة</CardTitle>
            <Badge status="completed">0</Badge>
          </CardHeader>
          <CardBody>
            <p>لا توجد بيانات في مرحلة Sprint 0.</p>
          </CardBody>
        </Card>
      </div>
    </AppShell>
  );
}
