using AutoMapper;
using Eap.Application.Acknowledgments.Models;
using Eap.Domain.Acknowledgment;

namespace Eap.Application.Acknowledgments.Mappings;

/// <summary>
/// AutoMapper profile for the acknowledgment management module. Kept explicit
/// so domain types never leak across the application boundary.
/// </summary>
public sealed class AcknowledgmentMappingProfile : Profile
{
    public AcknowledgmentMappingProfile()
    {
        CreateMap<AcknowledgmentVersion, AcknowledgmentVersionSummaryDto>();

        CreateMap<AcknowledgmentVersion, AcknowledgmentVersionDetailDto>();

        CreateMap<AcknowledgmentDefinition, AcknowledgmentDefinitionDetailDto>()
            .ForMember(d => d.Versions, opt => opt.MapFrom(src => src.Versions
                .OrderByDescending(v => v.VersionNumber)));

        CreateMap<AcknowledgmentDefinition, AcknowledgmentDefinitionSummaryDto>()
            .ForMember(d => d.VersionsCount, opt => opt.MapFrom(src => src.Versions.Count))
            .ForMember(d => d.CurrentVersionNumber, opt => opt.MapFrom(src =>
                src.CurrentAcknowledgmentVersionId == null
                    ? (int?)null
                    : src.Versions
                        .Where(v => v.Id == src.CurrentAcknowledgmentVersionId)
                        .Select(v => (int?)v.VersionNumber)
                        .FirstOrDefault()));
    }
}
