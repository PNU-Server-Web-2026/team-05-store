using System.Net;
using System.Text.Json;
using OnlineStore.Api.Tests.Infrastructure;

namespace OnlineStore.Api.Tests.Features.Health;

public sealed class HealthTests(ApiFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task Get_Health_ReturnsOkWithHealthyStatus()
    {
        var response = await Client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

        var content = await response.Content.ReadAsStringAsync();
        using var jsonDoc = JsonDocument.Parse(content);

        var root = jsonDoc.RootElement;
        Assert.True(root.TryGetProperty("status", out var statusProp));
        Assert.Equal("Healthy", statusProp.GetString());
    }
}
