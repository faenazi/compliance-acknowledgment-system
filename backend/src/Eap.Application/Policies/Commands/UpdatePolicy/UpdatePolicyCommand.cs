using Eap.Application.Policies.Models;
using MediatR;

namespace Eap.Application.Policies.Commands.UpdatePolicy;

/// <summary>Updates mutable master-metadata fields on an existing policy.</summary>
public sealed record UpdatePolicyCommand(
    Guid PolicyId,
    string Title,
    string OwnerDepartment,
    string? Category,
    string? Description) : IRequest<PolicyDetailDto>;
