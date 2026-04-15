import { forwardRef, type ButtonHTMLAttributes } from "react";
import { cn } from "@/lib/utils/cn";

type ButtonVariant = "primary" | "secondary" | "ghost";
type ButtonSize = "sm" | "md" | "lg";

interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: ButtonVariant;
  size?: ButtonSize;
}

const base =
  "inline-flex items-center justify-center gap-2 rounded-[10px] font-medium transition-colors disabled:pointer-events-none disabled:cursor-not-allowed";

const variants: Record<ButtonVariant, string> = {
  primary:
    "bg-[var(--color-brand-primary)] text-white border border-[var(--color-brand-primary)] hover:bg-[#24306A] disabled:bg-[#C7CEE8] disabled:border-[#C7CEE8]",
  secondary:
    "bg-white text-[var(--color-brand-primary)] border border-[#C7D2FE] hover:bg-[var(--color-surface-subtle)]",
  ghost:
    "bg-transparent text-[var(--color-brand-primary)] hover:bg-[rgba(44,58,130,0.06)] border border-transparent",
};

const sizes: Record<ButtonSize, string> = {
  sm: "h-9 px-3 text-sm",
  md: "h-11 px-4 text-sm",
  lg: "h-13 px-5 text-base",
};

export const Button = forwardRef<HTMLButtonElement, ButtonProps>(function Button(
  { variant = "primary", size = "md", className, ...rest },
  ref,
) {
  return (
    <button
      ref={ref}
      className={cn(base, variants[variant], sizes[size], className)}
      {...rest}
    />
  );
});
