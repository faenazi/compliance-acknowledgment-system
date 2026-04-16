using Eap.Application.Features.System.Echo;
using MediatR;

namespace Eap.Api.Endpoints;

public static class SystemEndpoints
{
    public static IEndpointRouteBuilder MapSystemEndpoints(this IEndpointRouteBuilder app)
    {
        var systemGroup = app.MapGroup("/api/v1/system").WithTags("System");

        systemGroup.MapGet("/echo", async (string message, ISender sender, CancellationToken cancellationToken) =>
            {
                var response = await sender.Send(new EchoQuery(message), cancellationToken);
                return Results.Ok(response);
            })
            .WithName("GetEcho")
            .WithSummary("Echo a message")
            .Produces<EchoResponse>();

        return app;
    }
}
