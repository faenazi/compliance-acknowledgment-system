/**
 * Frontend-facing contracts mirroring the Eap.Application.Acknowledgments DTOs.
 * Kept in sync with the backend on purpose — enums use numeric codes so the
 * API payload and the UI agree on the exact same state.
 */

export enum ActionType {
  SimpleAcknowledgment = 0,
  AcknowledgmentWithCommitment = 1,
  FormBasedDisclosure = 2,
}

/**
 * Mirrors Eap.Domain.Acknowledgment.RecurrenceModel. Publishing is blocked
 * while the model is Unspecified (BR-033).
 */
export enum RecurrenceModel {
  Unspecified = 0,
  OnboardingOnly = 1,
  Annual = 2,
  OnboardingAndAnnual = 3,
  OnChange = 4,
  EventDriven = 5,
}

export enum AcknowledgmentStatus {
  Draft = 0,
  Published = 1,
  Archived = 2,
}

export enum AcknowledgmentVersionStatus {
  Draft = 0,
  Published = 1,
  Superseded = 2,
  Archived = 3,
}

export interface AcknowledgmentVersionSummary {
  id: string;
  acknowledgmentDefinitionId: string;
  versionNumber: number;
  versionLabel: string | null;
  policyVersionId: string;
  actionType: ActionType;
  recurrenceModel: RecurrenceModel;
  status: AcknowledgmentVersionStatus;
  startDate: string | null;
  dueDate: string | null;
  publishedAtUtc: string | null;
  archivedAtUtc: string | null;
}

export interface AcknowledgmentVersionDetail {
  id: string;
  acknowledgmentDefinitionId: string;
  versionNumber: number;
  versionLabel: string | null;
  policyVersionId: string;
  actionType: ActionType;
  recurrenceModel: RecurrenceModel;
  summary: string | null;
  commitmentText: string | null;
  startDate: string | null;
  dueDate: string | null;
  status: AcknowledgmentVersionStatus;
  publishedAtUtc: string | null;
  publishedBy: string | null;
  archivedAtUtc: string | null;
  archivedBy: string | null;
  supersededByAcknowledgmentVersionId: string | null;
  createdAtUtc: string;
  createdBy: string | null;
  updatedAtUtc: string | null;
  updatedBy: string | null;
}

export interface AcknowledgmentDefinitionSummary {
  id: string;
  title: string;
  ownerDepartment: string;
  defaultActionType: ActionType;
  status: AcknowledgmentStatus;
  currentAcknowledgmentVersionId: string | null;
  currentVersionNumber: number | null;
  versionsCount: number;
  createdAtUtc: string;
  updatedAtUtc: string | null;
}

export interface AcknowledgmentDefinitionDetail {
  id: string;
  title: string;
  ownerDepartment: string;
  defaultActionType: ActionType;
  description: string | null;
  status: AcknowledgmentStatus;
  currentAcknowledgmentVersionId: string | null;
  versions: AcknowledgmentVersionSummary[];
  createdAtUtc: string;
  createdBy: string | null;
  updatedAtUtc: string | null;
  updatedBy: string | null;
}

export interface AcknowledgmentListParams {
  page?: number;
  pageSize?: number;
  search?: string;
  status?: AcknowledgmentStatus;
  ownerDepartment?: string;
  actionType?: ActionType;
}

export interface CreateAcknowledgmentDefinitionInput {
  title: string;
  ownerDepartment: string;
  defaultActionType: ActionType;
  description?: string | null;
}

export type UpdateAcknowledgmentDefinitionInput = CreateAcknowledgmentDefinitionInput;

export interface CreateAcknowledgmentVersionInput {
  policyVersionId: string;
  actionType: ActionType;
  recurrenceModel: RecurrenceModel;
  versionLabel?: string | null;
  summary?: string | null;
  commitmentText?: string | null;
  startDate?: string | null;
  dueDate?: string | null;
}

export type UpdateAcknowledgmentVersionDraftInput = CreateAcknowledgmentVersionInput;

/** Payload for PUT /versions/:id/recurrence (BR-046). */
export interface SetRecurrenceInput {
  recurrenceModel: RecurrenceModel;
  startDate?: string | null;
  dueDate?: string | null;
}
