using System.Reflection;
using Eap.Application.Common.Behaviors;
using Eap.Application.Identity.Abstractions;
using Eap.Application.Identity.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Eap.Application;

/// <summary>
/// Registers Application-layer services: MediatR, FluentValidation, AutoMapper,
/// cross-cutting pipeline behaviors, and per-feature application services.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);

        services.AddAutoMapper(cfg => cfg.AddMaps(assembly));

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // Application-layer services
        services.AddScoped<IUserProvisioner, UserProvisioner>();
        services.TryAddSingleton(TimeProvider.System);

        return services;
    }
}
