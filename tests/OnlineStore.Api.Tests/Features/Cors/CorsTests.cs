using Microsoft.AspNetCore.Hosting;

namespace OnlineStore.Api.Tests.Features.Cors;

public class CorsTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private const string AllowedOrigin = "http://localhost:5173";
    private const string DeniedOrigin = "https://example.com";

    private readonly HttpClient _client = factory
        .WithWebHostBuilder(builder => builder.UseEnvironment("Development"))
        .CreateClient();

    [Fact]
    public async Task Preflight_FromAllowedOrigin_ReturnsCorsHeaders()
    {
        using var request = CreatePreflightRequest(AllowedOrigin);

        var response = await _client.SendAsync(request);

        Assert.True(response.Headers.TryGetValues(
            "Access-Control-Allow-Origin",
            out var allowedOrigins));
        Assert.Contains(AllowedOrigin, allowedOrigins);
        Assert.True(response.Headers.Contains("Access-Control-Allow-Methods"));
        Assert.True(response.Headers.Contains("Access-Control-Allow-Headers"));
    }

    [Fact]
    public async Task Preflight_FromDeniedOrigin_DoesNotReturnCorsHeaders()
    {
        using var request = CreatePreflightRequest(DeniedOrigin);

        var response = await _client.SendAsync(request);

        Assert.DoesNotContain(
            response.Headers,
            header => header.Key.StartsWith("Access-Control-Allow-", StringComparison.OrdinalIgnoreCase));
    }

    private static HttpRequestMessage CreatePreflightRequest(string origin)
    {
        var request = new HttpRequestMessage(HttpMethod.Options, "/api/ping");
        request.Headers.Add("Origin", origin);
        request.Headers.Add("Access-Control-Request-Method", "POST");
        request.Headers.Add("Access-Control-Request-Headers", "Content-Type,Authorization");
        return request;
    }
}
