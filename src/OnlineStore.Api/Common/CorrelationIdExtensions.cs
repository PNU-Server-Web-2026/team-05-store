namespace OnlineStore.Api.Common;

/// <summary>
/// Реєстрація middleware correlation ID на початку HTTP-конвеєра.
/// </summary>
public static class CorrelationIdExtensions
{
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
        => app.UseMiddleware<CorrelationIdMiddleware>();
}
