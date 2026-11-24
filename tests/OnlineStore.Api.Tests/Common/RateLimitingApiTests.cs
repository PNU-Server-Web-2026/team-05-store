using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using OnlineStore.Api.Tests.Infrastructure;

namespace OnlineStore.Api.Tests.Common;

public sealed class RateLimitingApiTests(ApiFactory factory) : IntegrationTestBase(factory)
{
    private const int PermitLimit = 3;

    [Fact]
    public async Task Ping_WhenLimitExceeded_Returns429WithRetryAfterAndProblemDetails()
    {
        using var limitedFactory = Factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("RateLimiting:PermitLimit", PermitLimit.ToString());
            builder.UseSetting("RateLimiting:WindowSeconds", "60");
        });
        var client = limitedFactory.CreateClient();

        for (var i = 0; i < PermitLimit; i++)
        {
            var allowed = await client.GetAsync("/api/ping");
            Assert.Equal(HttpStatusCode.OK, allowed.StatusCode);
        }

        var rejected = await client.GetAsync("/api/ping");

        Assert.Equal(HttpStatusCode.TooManyRequests, rejected.StatusCode);
        Assert.True(rejected.Headers.RetryAfter?.Delta is { } retryAfter && retryAfter > TimeSpan.Zero);

        await using var stream = await rejected.Content.ReadAsStreamAsync();
        using var document = await JsonDocument.ParseAsync(stream);
        Assert.Equal(StatusCodes.Status429TooManyRequests, document.RootElement.GetProperty("status").GetInt32());
    }

    [Fact]
    public async Task Health_AfterPingLimit_StillReturnsOk()
    {
        using var limitedFactory = Factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("RateLimiting:PermitLimit", PermitLimit.ToString());
            builder.UseSetting("RateLimiting:WindowSeconds", "60");
        });
        var client = limitedFactory.CreateClient();

        for (var i = 0; i < PermitLimit + 1; i++)
        {
            await client.GetAsync("/api/ping");
        }

        var health = await client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, health.StatusCode);
    }
}
