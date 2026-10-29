using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using OnlineStore.Api.Tests.Infrastructure;

namespace OnlineStore.Api.Tests.Features.OpenApi;

public sealed class OpenApiDocumentationTests(ApiFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task Development_OpenApiDocument_HasInfoAndXmlComments()
    {
        await using var customized = Factory.WithWebHostBuilder(builder => builder.UseEnvironment("Development"));
        var client = customized.CreateClient();

        var response = await client.GetAsync("/openapi/v1.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await using var stream = await response.Content.ReadAsStreamAsync();
        using var document = await JsonDocument.ParseAsync(stream);
        var root = document.RootElement;
        var info = root.GetProperty("info");

        Assert.Equal("Online Store / Warehouse API", info.GetProperty("title").GetString());
        Assert.Equal("v1", info.GetProperty("version").GetString());
        Assert.Contains("склад", info.GetProperty("description").GetString(), StringComparison.OrdinalIgnoreCase);

        var pingSummary = root
            .GetProperty("paths")
            .GetProperty("/api/ping")
            .GetProperty("get")
            .GetProperty("summary")
            .GetString();
        Assert.Contains("pong", pingSummary, StringComparison.OrdinalIgnoreCase);

        var pingResponseDescription = root
            .GetProperty("components")
            .GetProperty("schemas")
            .GetProperty("PingResponse")
            .GetProperty("description")
            .GetString();
        Assert.Contains("ping", pingResponseDescription, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Development_ScalarUi_ReturnsHtml()
    {
        await using var customized = Factory.WithWebHostBuilder(builder => builder.UseEnvironment("Development"));
        var client = customized.CreateClient();

        var response = await client.GetAsync("/scalar");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("html", response.Content.Headers.ContentType?.MediaType, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Production_DoesNotPublishOpenApiOrScalar()
    {
        await using var customized = Factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production"));
        var client = customized.CreateClient();

        var openApi = await client.GetAsync("/openapi/v1.json");
        var scalar = await client.GetAsync("/scalar");

        Assert.Equal(HttpStatusCode.NotFound, openApi.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, scalar.StatusCode);
    }
}
