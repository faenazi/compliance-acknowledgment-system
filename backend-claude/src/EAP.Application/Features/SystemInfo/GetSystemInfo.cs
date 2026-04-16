using MediatR;

namespace EAP.Application.Features.SystemInfo;

public sealed record GetSystemInfoQuery : IRequest<GetSystemInfoResponse>;

public sealed record GetSystemInfoResponse
{
    public string SystemName { get; init; } = string.Empty;
    public string Version { get; init; } = string.Empty;
    public DateTime ServerTime { get; init; }
}

public sealed class GetSystemInfoHandler : IRequestHandler<GetSystemInfoQuery, GetSystemInfoResponse>
{
    public Task<GetSystemInfoResponse> Handle(GetSystemInfoQuery request, CancellationToken cancellationToken)
    {
        var response = new GetSystemInfoResponse
        {
            SystemName = "Enterprise Acknowledgment Platform",
            Version = "1.0.0",
            ServerTime = DateTime.UtcNow
        };

        return Task.FromResult(response);
    }
}
