using Eap.Application.Common.Models;
using Eap.Application.Policies.Commands.ArchivePolicy;
using Eap.Application.Policies.Commands.CreatePolicy;
using Eap.Application.Policies.Commands.UpdatePolicy;
using Eap.Application.Policies.Models;
using Eap.Application.Policies.Queries.GetPolicyById;
using Eap.Application.Policies.Queries.ListPolicies;
using Eap.Domain.Identity;
using Eap.Domain.Policy;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eap.Api.Features.Policies;

/// <summary>
/// Policy master endpoints. Controllers remain thin — every request is delegated
/// to a MediatR command/query so business logic stays in the Application layer.
/// Authoring requires <see cref="SystemRoles.PolicyManager"/>; read is allowed to
/// any authenticated admin-adjacent role (narrowed further in frontend).
/// </summary>
[ApiController]
[Route("api/policies")]
[Authorize]
public sealed class PoliciesController : ControllerBase
{
    private readonly IMediator _mediator;

    public PoliciesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<PolicySummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<PolicySummaryDto>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] PolicyStatus? status = null,
        [FromQuery] string? ownerDepartment = null,
        [FromQuery] string? category = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new ListPoliciesQuery(page, pageSize, search, status, ownerDepartment, category),
            cancellationToken);
        return Ok(result);
    }

    [HttpGet("{policyId:guid}")]
    [ProducesResponseType(typeof(PolicyDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PolicyDetailDto>> GetById(
        Guid policyId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetPolicyByIdQuery(policyId), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = SystemRoles.PolicyManager + "," + SystemRoles.SystemAdministrator)]
    [ProducesResponseType(typeof(PolicyDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PolicyDetailDto>> Create(
        [FromBody] CreatePolicyRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CreatePolicyCommand(
                request.PolicyCode,
                request.Title,
                request.OwnerDepartment,
                request.Category,
                request.Description),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { policyId = result.Id }, result);
    }

    [HttpPut("{policyId:guid}")]
    [Authorize(Roles = SystemRoles.PolicyManager + "," + SystemRoles.SystemAdministrator)]
    [ProducesResponseType(typeof(PolicyDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PolicyDetailDto>> Update(
        Guid policyId,
        [FromBody] UpdatePolicyRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdatePolicyCommand(
                policyId,
                request.Title,
                request.OwnerDepartment,
                request.Category,
                request.Description),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("{policyId:guid}/archive")]
    [Authorize(Roles = SystemRoles.PolicyManager + "," + SystemRoles.SystemAdministrator)]
    [ProducesResponseType(typeof(PolicyDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PolicyDetailDto>> Archive(
        Guid policyId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ArchivePolicyCommand(policyId), cancellationToken);
        return Ok(result);
    }
}
