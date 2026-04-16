namespace EAP.Domain.Entities;

public class Policy
{
    public Guid PolicyId { get; set; }
    public string PolicyCode { get; set; } = string.Empty;
    public string PolicyTitle { get; set; } = string.Empty;
    public string? OwnerDepartment { get; set; }
    public string? Category { get; set; }
    public string Status { get; set; } = "Draft";
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}
