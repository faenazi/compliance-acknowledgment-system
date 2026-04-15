import { actionTypeLabel } from "@/lib/acknowledgments/labels";
import { ActionType } from "@/lib/acknowledgments/types";

/**
 * Lightweight pill for the action type of a definition/version. Does not
 * use the status badge palette — this is a semantic categorization, not a
 * lifecycle state.
 */
export function ActionTypeBadge({ actionType }: { actionType: ActionType }) {
  return (
    <span
      className="inline-flex h-7 items-center rounded-full border border-[var(--color-border-default)] bg-[var(--color-surface-subtle)] px-[10px] text-xs font-medium text-[var(--color-text-secondary)]"
    >
      {actionTypeLabel[actionType]}
    </span>
  );
}
