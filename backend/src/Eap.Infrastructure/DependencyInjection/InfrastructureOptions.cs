namespace Eap.Infrastructure.DependencyInjection;

public sealed class InfrastructureOptions
{
    public const string SectionName = "Infrastructure";
    public bool EnableSensitiveDataLogging { get; init; }
}
