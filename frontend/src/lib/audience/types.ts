/**
 * Frontend contracts for the audience-targeting module. Enum codes match the
 * backend DTOs exactly so JSON is round-trippable without client-side mapping.
 */

export enum AudienceRuleType {
  AllUsers = 0,
  Department = 1,
  AdGroup = 2,
  User = 3,
}

export enum AudienceType {
  AllUsers = 0,
  Departments = 1,
  AdGroups = 2,
  Mixed = 3,
}

export interface AudienceRule {
  id: string;
  ruleType: AudienceRuleType;
  ruleValue: string | null;
  isExclusion: boolean;
  sortOrder: number;
}

export interface AudienceDefinition {
  id: string;
  acknowledgmentVersionId: string;
  audienceType: AudienceType;
  inclusionRules: AudienceRule[];
  exclusionRules: AudienceRule[];
  createdAtUtc: string;
  createdBy: string | null;
  updatedAtUtc: string | null;
  updatedBy: string | null;
}

export interface AudienceRuleInput {
  ruleType: AudienceRuleType;
  ruleValue?: string | null;
}

export interface ConfigureAudienceInclusionInput {
  rules: AudienceRuleInput[];
}

export interface ConfigureAudienceExclusionsInput {
  rules: AudienceRuleInput[];
}

export interface AudiencePreviewUser {
  userId: string;
  username: string;
  displayName: string;
  department: string | null;
}

export interface AudiencePreview {
  estimatedUserCount: number;
  inclusionMatchedCount: number;
  exclusionMatchedCount: number;
  sampleUsers: AudiencePreviewUser[];
}
