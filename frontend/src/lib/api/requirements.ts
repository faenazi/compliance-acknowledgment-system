import { apiClient } from "@/lib/api/client";
import type {
  GenerateRequirementsInput,
  RequirementGenerationSummary,
  UserActionRequirement,
  UserActionRequirementStatus,
} from "@/lib/requirements/types";

export async function listRequirementsForVersion(
  definitionId: string,
  versionId: string,
  params: { status?: UserActionRequirementStatus; currentOnly?: boolean } = {},
): Promise<UserActionRequirement[]> {
  const { data } = await apiClient.get<UserActionRequirement[]>(
    `/api/acknowledgments/${definitionId}/versions/${versionId}/requirements`,
    { params },
  );
  return data;
}

export async function generateRequirementsForVersion(
  definitionId: string,
  versionId: string,
  input: GenerateRequirementsInput = {},
): Promise<RequirementGenerationSummary> {
  const { data } = await apiClient.post<RequirementGenerationSummary>(
    `/api/acknowledgments/${definitionId}/versions/${versionId}/requirements/generate`,
    input,
  );
  return data;
}
