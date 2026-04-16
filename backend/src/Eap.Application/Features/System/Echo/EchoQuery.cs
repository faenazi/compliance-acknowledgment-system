using MediatR;

namespace Eap.Application.Features.System.Echo;

public sealed record EchoQuery(string Message) : IRequest<EchoResponse>;

public sealed record EchoResponse(string Message, DateTime ServerTimeUtc);
