using Microsoft.Extensions.Time.Testing;
using OnlineStore.Api.Features.Ping;

namespace OnlineStore.Api.Tests.Features.Ping;

/// <summary>
/// Приклад unit-тесту з FakeTimeProvider: час можна фіксувати і перемотувати.
/// </summary>
public sealed class PingTimeProviderTests
{
    [Fact]
    public void Get_UsesTimeFromProvider_AndReflectsAdvance()
    {
        var start = new DateTimeOffset(2026, 3, 15, 10, 0, 0, TimeSpan.Zero);
        var timeProvider = new FakeTimeProvider(start);
        var sut = new PingService(timeProvider);

        var first = sut.Get();
        Assert.Equal("pong", first.Message);
        Assert.Equal(start.UtcDateTime, first.Utc);

        timeProvider.Advance(TimeSpan.FromHours(48));

        var second = sut.Get();
        Assert.Equal(start.AddHours(48).UtcDateTime, second.Utc);
    }
}
