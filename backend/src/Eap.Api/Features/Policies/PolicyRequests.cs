using System.ComponentModel.DataAnnotations;

namespace Eap.Api.Features.Policies;

/// <summary>API contract for creating a policy.</summary>
public sealed class CreatePolicyRequest
{
    [Required]
    public string PolicyCode { get; set; } = string.Empty;

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string OwnerDepartment { get; set; } = string.Empty;

    public string? Category { get; set; }

    public string? Description { get; set; }
}

/// <summary>API contract for updating a policy's mutable metadata.</summary>
public sealed class UpdatePolicyRequest
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string OwnerDepartment { get; set; } = string.Empty;

    public string? Category { get; set; }

    public string? Description { get; set; }
}

/// <summary>API contract for creating a new draft version on a policy.</summary>
public sealed class CreatePolicyVersionRequest
{
    public string? VersionLabel { get; set; }

    public DateOnly? EffectiveDate { get; set; }

    public string? Summary { get; set; }
}

/// <summary>API contract for updating fields on a draft version.</summary>
public sealed class UpdatePolicyVersionDraftRequest
{
    public string? VersionLabel { get; set; }

    public DateOnly? EffectiveDate { get; set; }

    public string? Summary { get; set; }
}
