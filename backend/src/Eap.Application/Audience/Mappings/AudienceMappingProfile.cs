using AutoMapper;
using Eap.Application.Audience.Models;
using Eap.Domain.Audience;

namespace Eap.Application.Audience.Mappings;

public sealed class AudienceMappingProfile : Profile
{
    public AudienceMappingProfile()
    {
        CreateMap<AudienceRule, AudienceRuleDto>();

        CreateMap<AudienceDefinition, AudienceDefinitionDto>()
            .ForMember(d => d.InclusionRules, opt => opt.MapFrom(src =>
                src.Rules.Where(r => !r.IsExclusion).OrderBy(r => r.SortOrder)))
            .ForMember(d => d.ExclusionRules, opt => opt.MapFrom(src =>
                src.Rules.Where(r => r.IsExclusion).OrderBy(r => r.SortOrder)));
    }
}
