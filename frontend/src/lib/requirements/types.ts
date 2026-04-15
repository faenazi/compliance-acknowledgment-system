export enum UserActionRequirementStatus {
  Pending = 0,
  Completed = 1,
  Overdue = 2,
  Cancelled = 3,
}

export interface UserActionRequirement {
  id: string;
  userId: string;
  acknowledgmentVersionId: string;
  cycleReference: string;
  recurrenceInstanceDate: string | null;
  dueDate: string | null;
  assignedAtUtc: string;
  completedAtUtc: string | null;
  status: UserActionRequirementStatus;
  isCurrent: boolean;
}

export interface RequirementGenerationSummary {
  acknowledgmentVersionId: string;
  cycleReference: string;
  createdCount: number;
  skippedCount: number;
  cancelledCount: number;
  generatedAtUtc: string;
}

export interface GenerateRequirementsInput {
  cycleReference?: string | null;
}
