/**
 * Frontend-facing contracts for the admin portal APIs (Sprint 7).
 * Mirrors the Eap.Application.Admin.Models DTOs.
 */

import type { ActionType, RecurrenceModel } from "@/lib/acknowledgments/types";
import type { UserActionRequirementStatus } from "@/lib/requirements/types";

export interface AdminDashboardDto {
  activePolicies: number;
  activeAcknowledgments: number;
  pendingUserActions: number;
  overdueUserActions: number;
  completedUserActions: number;
  totalUserActions: number;
  completionRate: number;
  recentlyPublishedPolicies: RecentlyPublishedItemDto[];
  recentlyPublishedAcknowledgments: RecentlyPublishedItemDto[];
}

export interface RecentlyPublishedItemDto {
  id: string;
  versionId: string;
  title: string;
  ownerDepartment: string;
  versionNumber: number;
  publishedAtUtc: string;
}

export interface AdminRequirementSummaryDto {
  requirementId: string;
  userId: string;
  userDisplayName: string;
  userDepartment: string;
  acknowledgmentDefinitionId: string;
  acknowledgmentVersionId: string;
  actionTitle: string;
  actionType: ActionType;
  recurrenceModel: RecurrenceModel;
  versionNumber: number;
  policyTitle: string;
  status: UserActionRequirementStatus;
  cycleReference: string;
  assignedAtUtc: string;
  dueDate: string | null;
  completedAtUtc: string | null;
  isCurrent: boolean;
}

export interface AdminRequirementDetailDto {
  requirementId: string;
  status: UserActionRequirementStatus;
  dueDate: string | null;
  assignedAtUtc: string;
  completedAtUtc: string | null;
  cycleReference: string;
  isCurrent: boolean;

  userId: string;
  userDisplayName: string;
  userDepartment: string;
  userEmail: string | null;

  acknowledgmentDefinitionId: string;
  acknowledgmentVersionId: string;
  actionTitle: string;
  actionType: ActionType;
  recurrenceModel: RecurrenceModel;
  ownerDepartment: string;
  versionNumber: number;

  policyId: string;
  policyVersionId: string;
  policyTitle: string;
  policyVersionNumber: number;

  submissionId: string | null;
  submittedAtUtc: string | null;
  isLateSubmission: boolean | null;
}

export interface AdminSubmissionDetailDto {
  submissionId: string;
  submittedAtUtc: string;
  isLateSubmission: boolean;

  userId: string;
  userDisplayName: string;
  userDepartment: string;

  acknowledgmentDefinitionId: string;
  acknowledgmentVersionId: string;
  actionTitle: string;
  actionDescription: string | null;
  actionType: ActionType;
  ownerDepartment: string;
  versionNumber: number;
  commitmentText: string | null;

  policyTitle: string;
  policyVersionNumber: number;
  policyVersionLabel: string | null;

  requirementId: string | null;
  cycleReference: string | null;

  submissionJson: string;
  formDefinitionSnapshotJson: string | null;
  fieldValues: AdminFieldValueDto[] | null;
}

export interface AdminFieldValueDto {
  id: string;
  fieldKey: string;
  fieldLabel: string;
  fieldType: string;
  valueText: string | null;
  valueNumber: number | null;
  valueDate: string | null;
  valueBoolean: boolean | null;
  valueJson: string | null;
}

export interface AdminRequirementsListParams {
  page?: number;
  pageSize?: number;
  status?: UserActionRequirementStatus;
  acknowledgmentDefinitionId?: string;
  policyId?: string;
  department?: string;
  recurrenceModel?: RecurrenceModel;
  dueDateFrom?: string;
  dueDateTo?: string;
  search?: string;
  currentOnly?: boolean;
}
