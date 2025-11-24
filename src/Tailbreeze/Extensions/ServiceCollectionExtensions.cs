using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tailbreeze.Services;

namespace Tailbreeze.Extensions;

/// <summary>
/// Extension methods for setting up Tailbreeze services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Tailbreeze services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    public static IServiceCollection AddTailbreeze(this IServiceCollection services)
    {
        return services.AddTailbreeze(_ => { });
    }

    /// <summary>
    /// Adds Tailbreeze services to the specified <see cref="IServiceCollection"/> with configuration.
    /// </summary>
    public static IServiceCollection AddTailbreeze(
        this IServiceCollection services,
        Action<TailbreezeOptions> configure)
    {
        services.Configure(configure);

        services.TryAddSingleton<ITailwindCliService, TailwindCliService>();
        services.AddHostedService<TailwindHostedService>();

        return services;
    }
}
