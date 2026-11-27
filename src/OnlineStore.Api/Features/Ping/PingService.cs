namespace OnlineStore.Api.Features.Ping;

/// <summary>
/// Формує відповідь ping, беручи поточний час із <see cref="TimeProvider"/>.
/// </summary>
public sealed class PingService(TimeProvider timeProvider) : IPingService
{
    public PingResponse Get() =>
        new("pong", timeProvider.GetUtcNow().UtcDateTime);
}
