using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineStore.Api.Common;

namespace OnlineStore.Api.Tests.Common;

public sealed class RateLimitingTests
{
    private const int PermitLimit = 3;

    [Fact]
    public async Task Probe_WhenLimitExceeded_Returns429WithRetryAfterAndProblemDetails()
    {
        await using var app = await StartAsync();
        var client = app.GetTestClient();

        for (var i = 0; i < PermitLimit; i++)
        {
            var allowed = await client.GetAsync("/probe");
            Assert.Equal(HttpStatusCode.OK, allowed.StatusCode);
        }

        var rejected = await client.GetAsync("/probe");

        Assert.Equal(HttpStatusCode.TooManyRequests, rejected.StatusCode);
        Assert.True(rejected.Headers.RetryAfter?.Delta is { } retryAfter && retryAfter > TimeSpan.Zero);

        var mediaType = rejected.Content.Headers.ContentType?.MediaType;
        Assert.True(
            mediaType is "application/problem+json" or "application/json",
            $"Unexpected content type: {mediaType}");

        await using var stream = await rejected.Content.ReadAsStreamAsync();
        using var document = await JsonDocument.ParseAsync(stream);
        Assert.Equal(StatusCodes.Status429TooManyRequests, document.RootElement.GetProperty("status").GetInt32());
        Assert.Equal("Too Many Requests", document.RootElement.GetProperty("title").GetString());
    }

    [Fact]
    public async Task Health_IsNotRateLimited()
    {
        await using var app = await StartAsync();
        var client = app.GetTestClient();

        for (var i = 0; i < PermitLimit + 1; i++)
        {
            var probe = await client.GetAsync("/probe");
            if (i < PermitLimit)
            {
                Assert.Equal(HttpStatusCode.OK, probe.StatusCode);
            }
        }

        var health = await client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, health.StatusCode);
    }

    private static async Task<WebApplication> StartAsync()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["RateLimiting:PermitLimit"] = PermitLimit.ToString(),
            ["RateLimiting:WindowSeconds"] = "60"
        });
        builder.Services.AddProblemDetails();
        builder.Services.AddIpFixedWindowRateLimiting(builder.Configuration);

        var app = builder.Build();
        app.UseRateLimiter();
        app.MapGet("/probe", () => Results.Ok()).RequireIpRateLimiting();
        app.MapGet("/health", () => Results.Ok()).DisableRateLimiting();
        await app.StartAsync();
        return app;
    }
}
