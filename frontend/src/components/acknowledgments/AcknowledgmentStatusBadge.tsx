import { Badge } from "@/components/ui/badge";
import {
  acknowledgmentStatusBadge,
  acknowledgmentStatusLabel,
  acknowledgmentVersionStatusBadge,
  acknowledgmentVersionStatusLabel,
} from "@/lib/acknowledgments/labels";
import {
  AcknowledgmentStatus,
  AcknowledgmentVersionStatus,
} from "@/lib/acknowledgments/types";

export function AcknowledgmentStatusBadge({ status }: { status: AcknowledgmentStatus }) {
  return (
    <Badge status={acknowledgmentStatusBadge[status]}>
      {acknowledgmentStatusLabel[status]}
    </Badge>
  );
}

export function AcknowledgmentVersionStatusBadge({
  status,
}: {
  status: AcknowledgmentVersionStatus;
}) {
  return (
    <Badge status={acknowledgmentVersionStatusBadge[status]}>
      {acknowledgmentVersionStatusLabel[status]}
    </Badge>
  );
}
