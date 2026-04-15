import { Badge } from "@/components/ui/badge";
import {
  policyStatusBadge,
  policyStatusLabel,
  versionStatusBadge,
  versionStatusLabel,
} from "@/lib/policies/labels";
import { PolicyStatus, PolicyVersionStatus } from "@/lib/policies/types";

export function PolicyStatusBadge({ status }: { status: PolicyStatus }) {
  return <Badge status={policyStatusBadge[status]}>{policyStatusLabel[status]}</Badge>;
}

export function PolicyVersionStatusBadge({ status }: { status: PolicyVersionStatus }) {
  return <Badge status={versionStatusBadge[status]}>{versionStatusLabel[status]}</Badge>;
}
