using AutoMapper;
using Eap.Application.Forms.Models;
using Eap.Domain.Forms;

namespace Eap.Application.Forms.Mappings;

public sealed class FormMappingProfile : Profile
{
    public FormMappingProfile()
    {
        CreateMap<FieldOption, FieldOptionDto>();

        CreateMap<FormField, FormFieldDto>();

        CreateMap<FormDefinition, FormDefinitionDto>()
            .ForMember(d => d.Fields, opt => opt.MapFrom(src =>
                src.Fields.OrderBy(f => f.SortOrder)));

        CreateMap<UserSubmissionFieldValue, SubmissionFieldValueDto>();

        CreateMap<UserSubmission, UserSubmissionSummaryDto>();

        CreateMap<UserSubmission, UserSubmissionDetailDto>()
            .ForMember(d => d.FieldValues, opt => opt.MapFrom(src => src.FieldValues));
    }
}
