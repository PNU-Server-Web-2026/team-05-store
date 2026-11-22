using Microsoft.Extensions.Primitives;

namespace OnlineStore.Api.Common;

/// <summary>
/// Читає або генерує ідентифікатор запиту і прокидає його в заголовок відповіді та в logging scope.
/// </summary>
public sealed class CorrelationIdMiddleware
{
    public const string HeaderName = "X-Correlation-Id";
    public const string ItemKey = "CorrelationId";

    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;

    public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = ResolveCorrelationId(context.Request.Headers[HeaderName]);
        context.Items[ItemKey] = correlationId;

        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = correlationId;
            return Task.CompletedTask;
        });

        using (_logger.BeginScope(new Dictionary<string, object> { [ItemKey] = correlationId }))
        {
            await _next(context);
        }
    }

    private static string ResolveCorrelationId(StringValues headerValues)
    {
        var incoming = headerValues.FirstOrDefault();
        return string.IsNullOrWhiteSpace(incoming)
            ? Guid.NewGuid().ToString()
            : incoming.Trim();
    }
}
