using AutoMapper;
using Eap.Application.Requirements.Models;
using Eap.Domain.Requirements;

namespace Eap.Application.Requirements.Mappings;

public sealed class RequirementMappingProfile : Profile
{
    public RequirementMappingProfile()
    {
        CreateMap<UserActionRequirement, UserActionRequirementDto>();
    }
}
