using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace OnlineStore.Api.Tests.Features.Health;

public class HealthTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Get_Health_ReturnsOkWithHealthyStatus()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

        var content = await response.Content.ReadAsStringAsync();
        using var jsonDoc = JsonDocument.Parse(content);

        var root = jsonDoc.RootElement;
        Assert.True(root.TryGetProperty("status", out var statusProp));
        Assert.Equal("Healthy", statusProp.GetString());
    }
}
