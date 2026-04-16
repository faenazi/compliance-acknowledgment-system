using Eap.Application.Admin.Models;
using Eap.Application.Admin.Queries.GetAdminDashboard;
using Eap.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eap.Api.Features.Admin;

/// <summary>
/// Admin dashboard summary endpoint (Sprint 7). Restricted to admin-capable roles.
/// </summary>
[ApiController]
[Route("api/admin/dashboard")]
[Authorize(Roles = SystemRoles.SystemAdministrator + ","
    + SystemRoles.PolicyManager + ","
    + SystemRoles.AcknowledgmentManager + ","
    + SystemRoles.Publisher + ","
    + SystemRoles.ComplianceViewer + ","
    + SystemRoles.Auditor)]
public sealed class AdminDashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminDashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Returns the operational admin dashboard summary.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(AdminDashboardDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AdminDashboardDto>> GetDashboard(
        [FromQuery] int recentLimit = 5,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetAdminDashboardQuery(recentLimit), cancellationToken);
        return Ok(result);
    }
}
