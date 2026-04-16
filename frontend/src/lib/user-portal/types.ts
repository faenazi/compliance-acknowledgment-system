/**
 * Frontend-facing contracts for the user portal APIs (Sprint 6).
 * Mirrors the Eap.Application.UserPortal.Models DTOs.
 */

import type { ActionType, RecurrenceModel } from "@/lib/acknowledgments/types";
import type { FormDefinitionDto, SubmissionFieldValueDto } from "@/lib/forms/types";
import { UserActionRequirementStatus } from "@/lib/requirements/types";

export { UserActionRequirementStatus };

export interface MyDashboardDto {
  pendingCount: number;
  overdueCount: number;
  completedCount: number;
  pendingActions: MyActionSummaryDto[];
  recentlyCompleted: MyActionSummaryDto[];
}

export interface MyActionSummaryDto {
  requirementId: string;
  title: string;
  policyTitle: string;
  actionType: ActionType;
  recurrenceModel: RecurrenceModel;
  ownerDepartment: string;
  dueDate: string | null;
  status: UserActionRequirementStatus;
  assignedAtUtc: string;
  completedAtUtc: string | null;
  cycleReference: string;
}

export interface MyActionDetailDto {
  // Requirement
  requirementId: string;
  status: UserActionRequirementStatus;
  dueDate: string | null;
  assignedAtUtc: string;
  completedAtUtc: string | null;
  cycleReference: string;

  // Acknowledgment
  acknowledgmentVersionId: string;
  acknowledgmentDefinitionId: string;
  title: string;
  description: string | null;
  actionType: ActionType;
  recurrenceModel: RecurrenceModel;
  ownerDepartment: string;
  summary: string | null;
  commitmentText: string | null;
  startDate: string | null;

  // Policy
  policyVersionId: string;
  policyId: string;
  policyTitle: string;
  policySummary: string | null;
  policyVersionNumber: number;
  policyVersionLabel: string | null;
  hasPolicyDocument: boolean;

  // Form (if FormBasedDisclosure)
  formDefinition: FormDefinitionDto | null;

  // Existing submission
  submissionId: string | null;
  submittedAtUtc: string | null;
}

export interface MyHistoryItemDto {
  submissionId: string;
  requirementId: string;
  title: string;
  policyTitle: string;
  actionType: ActionType;
  versionNumber: number;
  submittedAtUtc: string;
  isLateSubmission: boolean;
  cycleReference: string;
}

export interface MySubmissionDetailDto {
  submissionId: string;
  requirementId: string;
  submittedAtUtc: string;
  isLateSubmission: boolean;

  // Context
  title: string;
  description: string | null;
  actionType: ActionType;
  ownerDepartment: string;
  versionNumber: number;
  commitmentText: string | null;

  // Policy
  policyTitle: string;
  policyVersionNumber: number;
  policyVersionLabel: string | null;

  // Submitted data
  submissionJson: string;
  formDefinitionSnapshotJson: string | null;
  fieldValues: SubmissionFieldValueDto[] | null;
}

export interface SubmissionResultDto {
  submissionId: string;
  requirementId: string;
  submittedAtUtc: string;
  requirementStatus: UserActionRequirementStatus;
  isLateSubmission: boolean;
}

export interface MyActionsListParams {
  page?: number;
  pageSize?: number;
  status?: UserActionRequirementStatus;
  search?: string;
}
