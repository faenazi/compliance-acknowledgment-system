using Eap.Application.Identity.Abstractions;
using Eap.Application.Requirements.Abstractions;
using Eap.Application.Requirements.Models;
using MediatR;

namespace Eap.Application.Requirements.Commands.GenerateRequirementsForVersion;

public sealed class GenerateRequirementsForVersionCommandHandler
    : IRequestHandler<GenerateRequirementsForVersionCommand, RequirementGenerationSummaryDto>
{
    private readonly IRequirementGenerator _generator;
    private readonly ICurrentUser _currentUser;

    public GenerateRequirementsForVersionCommandHandler(
        IRequirementGenerator generator,
        ICurrentUser currentUser)
    {
        _generator = generator;
        _currentUser = currentUser;
    }

    public Task<RequirementGenerationSummaryDto> Handle(
        GenerateRequirementsForVersionCommand request,
        CancellationToken cancellationToken)
    {
        return _generator.GenerateForVersionAsync(
            request.VersionId,
            request.CycleReference,
            _currentUser.Username,
            cancellationToken);
    }
}
