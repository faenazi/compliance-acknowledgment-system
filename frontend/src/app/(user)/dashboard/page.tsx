"use client";

import Link from "next/link";
import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { RequirementStatusBadge } from "@/components/user-portal/RequirementStatusBadge";
import { useSession } from "@/lib/auth/SessionProvider";
import { useMyDashboard } from "@/lib/user-portal/hooks";
import { actionTypeLabel } from "@/lib/acknowledgments/labels";
import { formatDateAr } from "@/lib/user-portal/labels";
import { UserActionRequirementStatus } from "@/lib/user-portal/types";

/**
 * Employee dashboard (Sprint 6). Shows summary counts and recent actions.
 */
export default function UserDashboardPage() {
  const { user } = useSession();
  const { data: dashboard, isLoading } = useMyDashboard();

  return (
    <>
      <section className="mb-8">
        <h1 className="text-3xl font-bold">
          {user ? `مرحباً، ${user.displayName}` : "لوحة الموظف"}
        </h1>
        <p className="mt-2 text-[var(--color-text-secondary)]">
          يمكنك متابعة الإقرارات المطلوبة والمكتملة من هنا.
        </p>
      </section>

      {/* Summary Cards */}
      <div className="mb-8 grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
        <SummaryCard
          title="المعلّقة"
          count={dashboard?.pendingCount ?? 0}
          status="pending"
          loading={isLoading}
        />
        <SummaryCard
          title="المتأخرة"
          count={dashboard?.overdueCount ?? 0}
          status="overdue"
          loading={isLoading}
        />
        <SummaryCard
          title="المكتملة"
          count={dashboard?.completedCount ?? 0}
          status="completed"
          loading={isLoading}
        />
      </div>

      {/* Pending Actions */}
      <section className="mb-8">
        <div className="mb-4 flex items-center justify-between">
          <h2 className="text-xl font-semibold">الإجراءات المطلوبة</h2>
          <Link href="/actions">
            <Button variant="ghost" size="sm">
              عرض الكل
            </Button>
          </Link>
        </div>

        {isLoading ? (
          <Card>
            <CardBody>
              <p className="text-sm text-[var(--color-text-tertiary)]">
                جاري التحميل…
              </p>
            </CardBody>
          </Card>
        ) : dashboard?.pendingActions.length === 0 ? (
          <Card>
            <CardBody>
              <p className="text-sm text-[var(--color-text-secondary)]">
                لا توجد إجراءات معلّقة حالياً. عمل ممتاز!
              </p>
            </CardBody>
          </Card>
        ) : (
          <div className="space-y-3">
            {dashboard?.pendingActions.map((action) => (
              <ActionRow key={action.requirementId} action={action} />
            ))}
          </div>
        )}
      </section>

      {/* Recently Completed */}
      {(dashboard?.recentlyCompleted.length ?? 0) > 0 && (
        <section>
          <div className="mb-4 flex items-center justify-between">
            <h2 className="text-xl font-semibold">المكتملة مؤخراً</h2>
            <Link href="/history">
              <Button variant="ghost" size="sm">
                السجل الكامل
              </Button>
            </Link>
          </div>
          <div className="space-y-3">
            {dashboard?.recentlyCompleted.map((action) => (
              <ActionRow key={action.requirementId} action={action} />
            ))}
          </div>
        </section>
      )}
    </>
  );
}

function SummaryCard({
  title,
  count,
  status,
  loading,
}: {
  title: string;
  count: number;
  status: "pending" | "overdue" | "completed";
  loading: boolean;
}) {
  return (
    <Card>
      <CardHeader>
        <CardTitle>{title}</CardTitle>
        <Badge status={status}>{loading ? "…" : count}</Badge>
      </CardHeader>
    </Card>
  );
}

function ActionRow({
  action,
}: {
  action: {
    requirementId: string;
    title: string;
    policyTitle: string;
    actionType: number;
    dueDate: string | null;
    status: UserActionRequirementStatus;
  };
}) {
  const isOverdue = action.status === UserActionRequirementStatus.Overdue;

  return (
    <Link href={`/actions/${action.requirementId}`}>
      <Card
        className={`transition-shadow hover:shadow-md ${isOverdue ? "border-[var(--color-status-overdue-border)]" : ""}`}
      >
        <div className="flex items-center justify-between gap-4">
          <div className="min-w-0 flex-1">
            <div className="flex items-center gap-2">
              <span className="text-sm font-semibold text-[var(--color-text-primary)] truncate">
                {action.title}
              </span>
              <RequirementStatusBadge status={action.status} />
            </div>
            <div className="mt-1 flex items-center gap-3 text-xs text-[var(--color-text-tertiary)]">
              <span>{action.policyTitle}</span>
              <span>·</span>
              <span>{actionTypeLabel[action.actionType as keyof typeof actionTypeLabel]}</span>
              {action.dueDate && (
                <>
                  <span>·</span>
                  <span className={isOverdue ? "text-[var(--color-status-overdue-text)] font-medium" : ""}>
                    الاستحقاق: {formatDateAr(action.dueDate)}
                  </span>
                </>
              )}
            </div>
          </div>
          <svg
            className="h-5 w-5 shrink-0 text-[var(--color-text-tertiary)] rtl:rotate-180"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
          </svg>
        </div>
      </Card>
    </Link>
  );
}
