using MediatR;

namespace Eap.Application.Identity.Commands.Logout;

public sealed record LogoutCommand : IRequest<Unit>;
