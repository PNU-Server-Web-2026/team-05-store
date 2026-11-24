using System.Globalization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace OnlineStore.Api.Common;

public static class RateLimitingExtensions
{
    public static IServiceCollection AddIpFixedWindowRateLimiting(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<RateLimitOptions>()
            .Bind(configuration.GetSection(RateLimitOptions.SectionName))
            .Validate(options => options.PermitLimit > 0 && options.WindowSeconds > 0,
                "RateLimiting:PermitLimit and RateLimiting:WindowSeconds must be greater than zero.")
            .ValidateOnStart();

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = WriteTooManyRequestsAsync;
            options.AddPolicy(RateLimitOptions.PolicyName, httpContext =>
            {
                var limits = httpContext.RequestServices
                    .GetRequiredService<IOptionsSnapshot<RateLimitOptions>>()
                    .Value;
                var partitionKey = httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous";

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey,
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = limits.PermitLimit,
                        Window = TimeSpan.FromSeconds(limits.WindowSeconds),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    });
            });
        });

        return services;
    }

    public static IEndpointConventionBuilder RequireIpRateLimiting(this IEndpointConventionBuilder endpoints)
        => endpoints.RequireRateLimiting(RateLimitOptions.PolicyName);

    private static async ValueTask WriteTooManyRequestsAsync(
        OnRejectedContext context,
        CancellationToken cancellationToken)
    {
        var httpContext = context.HttpContext;
        httpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

        var retryAfterSeconds = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter)
            ? Math.Max(1, (int)Math.Ceiling(retryAfter.TotalSeconds))
            : httpContext.RequestServices.GetRequiredService<IOptions<RateLimitOptions>>().Value.WindowSeconds;

        httpContext.Response.Headers.RetryAfter = retryAfterSeconds.ToString(CultureInfo.InvariantCulture);

        var problemDetails = httpContext.RequestServices.GetRequiredService<IProblemDetailsService>();
        await problemDetails.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status429TooManyRequests,
                Title = "Too Many Requests",
                Type = "https://tools.ietf.org/html/rfc6585#section-4",
                Detail = "Too many requests. Please retry later."
            }
        });
    }
}
