using System.ComponentModel.DataAnnotations;
using Eap.Domain.Acknowledgment;

namespace Eap.Api.Features.Acknowledgments;

/// <summary>API contract for creating a master acknowledgment definition.</summary>
public sealed class CreateAcknowledgmentDefinitionRequest
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string OwnerDepartment { get; set; } = string.Empty;

    [Required]
    public ActionType DefaultActionType { get; set; }

    public string? Description { get; set; }
}

/// <summary>API contract for updating an acknowledgment definition's mutable metadata.</summary>
public sealed class UpdateAcknowledgmentDefinitionRequest
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string OwnerDepartment { get; set; } = string.Empty;

    [Required]
    public ActionType DefaultActionType { get; set; }

    public string? Description { get; set; }
}

/// <summary>API contract for creating a new draft version on an acknowledgment definition.</summary>
public sealed class CreateAcknowledgmentVersionRequest
{
    [Required]
    public Guid PolicyVersionId { get; set; }

    [Required]
    public ActionType ActionType { get; set; }

    public string? VersionLabel { get; set; }

    public string? Summary { get; set; }

    public string? CommitmentText { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? DueDate { get; set; }
}

/// <summary>API contract for updating fields on a draft acknowledgment version.</summary>
public sealed class UpdateAcknowledgmentVersionDraftRequest
{
    [Required]
    public Guid PolicyVersionId { get; set; }

    [Required]
    public ActionType ActionType { get; set; }

    public string? VersionLabel { get; set; }

    public string? Summary { get; set; }

    public string? CommitmentText { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? DueDate { get; set; }
}
