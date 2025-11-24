using Microsoft.AspNetCore.Builder;
using Tailbreeze.Middleware;

namespace Tailbreeze.Extensions;

/// <summary>
/// Extension methods for adding Tailbreeze middleware.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds Tailwind CSS middleware to the application pipeline.
    /// </summary>
    public static IApplicationBuilder UseTailbreeze(this IApplicationBuilder app)
    {
        return app.UseMiddleware<TailwindMiddleware>();
    }
}
