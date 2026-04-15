using AutoMapper;
using Eap.Application.Policies.Models;
using Eap.Domain.Policy;

namespace Eap.Application.Policies.Mappings;

/// <summary>
/// AutoMapper profile for the policy management module. Mapping is kept
/// explicit so DTOs stay decoupled from domain entities; no EF types leak
/// through the application boundary.
/// </summary>
public sealed class PolicyMappingProfile : Profile
{
    public PolicyMappingProfile()
    {
        CreateMap<PolicyDocument, PolicyDocumentDto>();

        CreateMap<PolicyVersion, PolicyVersionSummaryDto>()
            .ForMember(d => d.HasDocument, opt => opt.MapFrom(src => src.HasDocument));

        CreateMap<PolicyVersion, PolicyVersionDetailDto>();

        CreateMap<Policy, PolicyDetailDto>()
            .ForMember(d => d.Versions, opt => opt.MapFrom(src => src.Versions
                .OrderByDescending(v => v.VersionNumber)));

        CreateMap<Policy, PolicySummaryDto>()
            .ForMember(d => d.VersionsCount, opt => opt.MapFrom(src => src.Versions.Count))
            .ForMember(d => d.CurrentVersionNumber, opt => opt.MapFrom(src =>
                src.CurrentPolicyVersionId == null
                    ? (int?)null
                    : src.Versions
                        .Where(v => v.Id == src.CurrentPolicyVersionId)
                        .Select(v => (int?)v.VersionNumber)
                        .FirstOrDefault()))
            .ForMember(d => d.CurrentEffectiveDate, opt => opt.MapFrom(src =>
                src.CurrentPolicyVersionId == null
                    ? null
                    : src.Versions
                        .Where(v => v.Id == src.CurrentPolicyVersionId)
                        .Select(v => v.EffectiveDate)
                        .FirstOrDefault()));
    }
}
