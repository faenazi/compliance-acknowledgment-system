import type { ReactNode } from "react";
import { cn } from "@/lib/utils/cn";

interface KpiCardProps {
  /** Arabic label displayed above the value. */
  label: string;
  /** The primary numeric or text value. */
  value: string | number;
  /** Optional icon or visual element rendered to the start. */
  icon?: ReactNode;
  /** Optional accent color class for the value text. */
  accentClass?: string;
  /** Optional subtitle below the value. */
  subtitle?: string;
  /** Optional click handler. */
  onClick?: () => void;
}

/**
 * Operational KPI card for the admin dashboard.
 * Follows docs/09-ux-ui/admin-portal-pages.md §23.1.
 * Kept intentionally simple: label, value, optional icon and subtitle.
 */
export function KpiCard({
  label,
  value,
  icon,
  accentClass,
  subtitle,
  onClick,
}: KpiCardProps) {
  const Component = onClick ? "button" : "div";

  return (
    <Component
      onClick={onClick}
      className={cn(
        "flex items-start gap-4 rounded-[14px] border border-[var(--color-border-default)] bg-white p-5 shadow-[var(--shadow-sm)] text-right",
        onClick && "cursor-pointer transition-shadow hover:shadow-[var(--shadow-md)]",
      )}
    >
      {icon ? (
        <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-[10px] bg-[var(--color-surface-subtle)] text-[var(--color-brand-primary)]">
          {icon}
        </div>
      ) : null}
      <div className="min-w-0 flex-1">
        <p className="text-xs font-medium text-[var(--color-text-tertiary)]">{label}</p>
        <p className={cn("mt-1 text-2xl font-bold leading-tight", accentClass ?? "text-[var(--color-text-primary)]")}>
          {value}
        </p>
        {subtitle ? (
          <p className="mt-1 text-xs text-[var(--color-text-secondary)]">{subtitle}</p>
        ) : null}
      </div>
    </Component>
  );
}
