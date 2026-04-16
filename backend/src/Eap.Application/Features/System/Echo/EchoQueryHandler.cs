using MediatR;

namespace Eap.Application.Features.System.Echo;

public sealed class EchoQueryHandler : IRequestHandler<EchoQuery, EchoResponse>
{
    public Task<EchoResponse> Handle(EchoQuery request, CancellationToken cancellationToken)
    {
        var response = new EchoResponse(
            request.Message.Trim(),
            DateTime.UtcNow);

        return Task.FromResult(response);
    }
}
