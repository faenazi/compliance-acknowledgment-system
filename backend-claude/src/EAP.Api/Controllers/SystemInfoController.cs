using EAP.Application.Features.SystemInfo;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EAP.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SystemInfoController : ControllerBase
{
    private readonly IMediator _mediator;

    public SystemInfoController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(GetSystemInfoResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSystemInfoQuery(), cancellationToken);
        return Ok(result);
    }
}
