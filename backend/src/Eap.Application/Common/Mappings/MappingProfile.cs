using AutoMapper;

namespace Eap.Application.Common.Mappings;

/// <summary>
/// Base AutoMapper profile for the Application layer.
/// Concrete modules add their own profiles; this type exists so that the
/// Application assembly has at least one profile to scan in Sprint 0.
/// </summary>
public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Mappings are added by each module/feature as it is introduced.
    }
}
