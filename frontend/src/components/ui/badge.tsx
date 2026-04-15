import type { HTMLAttributes } from "react";
import { cn } from "@/lib/utils/cn";
import { statusColors, type StatusKey } from "@/lib/tokens/design-tokens";

interface BadgeProps extends HTMLAttributes<HTMLSpanElement> {
  status: StatusKey;
}

/**
 * Status badge aligned with docs/09-ux-ui/design-system-tokens.md §4.4.
 */
export function Badge({ status, className, children, ...rest }: BadgeProps) {
  const palette = statusColors[status];
  return (
    <span
      className={cn(
        "inline-flex h-7 items-center rounded-full border px-[10px] text-xs font-semibold",
        className,
      )}
      style={{
        backgroundColor: palette.bg,
        color: palette.text,
        borderColor: palette.border,
      }}
      {...rest}
    >
      {children}
    </span>
  );
}
