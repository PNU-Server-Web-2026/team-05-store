namespace OnlineStore.Api.Features.Ping;

/// <summary>
/// Відповідь GET /api/ping.
/// </summary>
public sealed record PingResponse(string Message, DateTime Utc);
