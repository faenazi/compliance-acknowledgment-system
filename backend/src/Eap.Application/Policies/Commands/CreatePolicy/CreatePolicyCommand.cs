using Eap.Application.Policies.Models;
using MediatR;

namespace Eap.Application.Policies.Commands.CreatePolicy;

/// <summary>Creates a new master policy (no version yet).</summary>
public sealed record CreatePolicyCommand(
    string PolicyCode,
    string Title,
    string OwnerDepartment,
    string? Category,
    string? Description) : IRequest<PolicyDetailDto>;
