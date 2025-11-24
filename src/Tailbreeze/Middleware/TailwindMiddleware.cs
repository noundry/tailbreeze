using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Tailbreeze.Middleware;

/// <summary>
/// Middleware for serving Tailwind CSS files during development.
/// </summary>
public sealed class TailwindMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IOptions<TailbreezeOptions> _options;
    private readonly IHostEnvironment _environment;
    private readonly ILogger<TailwindMiddleware> _logger;

    public TailwindMiddleware(
        RequestDelegate next,
        IOptions<TailbreezeOptions> options,
        IHostEnvironment environment,
        ILogger<TailwindMiddleware> logger)
    {
        _next = next;
        _options = options;
        _environment = environment;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var opts = _options.Value;

        if (!opts.ServeViaMiddleware)
        {
            await _next(context);
            return;
        }

        if (!context.Request.Path.StartsWithSegments(opts.ServePath, StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        var cssFilePath = Path.Combine(_environment.ContentRootPath, "wwwroot", opts.OutputCssPath);

        if (!File.Exists(cssFilePath))
        {
            _logger.LogWarning("Tailwind CSS file not found at {Path}", cssFilePath);

            if (opts.UseCdnFallback ?? _environment.IsDevelopment())
            {
                await ServeCdnFallbackAsync(context);
                return;
            }

            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }

        context.Response.ContentType = "text/css; charset=utf-8";
        context.Response.Headers.CacheControl = _environment.IsDevelopment()
            ? "no-cache, no-store, must-revalidate"
            : "public, max-age=31536000, immutable";

        await context.Response.SendFileAsync(cssFilePath);
    }

    private async Task ServeCdnFallbackAsync(HttpContext context)
    {
        var version = TailwindVersion.Parse(_options.Value.TailwindVersion);
        var cdnUrl = version.IsV4
            ? "https://cdn.tailwindcss.com/4.0.0-alpha.32"
            : "https://cdn.tailwindcss.com";

        _logger.LogDebug("Serving Tailwind CSS from CDN: {Url}", cdnUrl);

        context.Response.StatusCode = StatusCodes.Status302Found;
        context.Response.Headers.Location = cdnUrl;

        await context.Response.WriteAsync($"/* Redirecting to Tailwind CDN: {cdnUrl} */");
    }
}
