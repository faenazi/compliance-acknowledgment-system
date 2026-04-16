/**
 * Frontend contracts for compliance APIs (Sprint 8).
 * Mirrors Eap.Application.Compliance.Models DTOs.
 */

import type { ActionType } from "@/lib/acknowledgments/types";
import type { UserActionRequirementStatus } from "@/lib/requirements/types";

export interface ComplianceDashboardDto {
  totalRequirements: number;
  completedRequirements: number;
  pendingRequirements: number;
  overdueRequirements: number;
  completionRate: number;
  complianceByDepartment: DepartmentComplianceDto[];
  complianceByAction: ActionComplianceDto[];
  topNonCompliantUsers: NonCompliantUserSummaryDto[];
}

export interface DepartmentComplianceDto {
  department: string;
  totalAssigned: number;
  completed: number;
  pending: number;
  overdue: number;
  completionRate: number;
}

export interface ActionComplianceDto {
  acknowledgmentDefinitionId: string;
  actionTitle: string;
  actionType: ActionType;
  ownerDepartment: string;
  totalAssigned: number;
  completed: number;
  pending: number;
  overdue: number;
  completionRate: number;
}

export interface NonCompliantUserSummaryDto {
  userId: string;
  displayName: string;
  department: string;
  email: string | null;
  pendingCount: number;
  overdueCount: number;
  totalNonCompliant: number;
}

export interface NonCompliantUserDetailDto {
  userId: string;
  displayName: string;
  department: string;
  email: string | null;
  requirementId: string;
  acknowledgmentDefinitionId: string;
  acknowledgmentVersionId: string;
  actionTitle: string;
  actionType: ActionType;
  status: UserActionRequirementStatus;
  dueDate: string | null;
  assignedAtUtc: string;
  cycleReference: string;
}

export interface ComplianceDashboardParams {
  department?: string;
  acknowledgmentDefinitionId?: string;
  policyId?: string;
  topNonCompliantLimit?: number;
}

export interface NonCompliantUsersParams {
  page?: number;
  pageSize?: number;
  department?: string;
  acknowledgmentDefinitionId?: string;
  policyId?: string;
  status?: UserActionRequirementStatus;
  search?: string;
}
