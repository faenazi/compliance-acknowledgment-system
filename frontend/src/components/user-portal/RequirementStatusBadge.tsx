import { Badge } from "@/components/ui/badge";
import { UserActionRequirementStatus } from "@/lib/user-portal/types";
import {
  requirementStatusBadge,
  requirementStatusLabel,
} from "@/lib/user-portal/labels";

interface Props {
  status: UserActionRequirementStatus;
  className?: string;
}

export function RequirementStatusBadge({ status, className }: Props) {
  return (
    <Badge status={requirementStatusBadge[status]} className={className}>
      {requirementStatusLabel[status]}
    </Badge>
  );
}
