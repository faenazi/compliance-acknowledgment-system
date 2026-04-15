import { AppShell } from "@/components/layout/AppShell";
import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";

/**
 * Login placeholder for Sprint 0.
 * Real LDAP / Active Directory flow is implemented in Sprint 1.
 */
export default function LoginPage() {
  return (
    <AppShell>
      <div className="mx-auto max-w-[480px]">
        <Card>
          <CardHeader>
            <CardTitle>تسجيل الدخول</CardTitle>
          </CardHeader>
          <CardBody>
            <p>
              سيتم تفعيل المصادقة عبر LDAP / Active Directory في Sprint 1.
            </p>
          </CardBody>
        </Card>
      </div>
    </AppShell>
  );
}
