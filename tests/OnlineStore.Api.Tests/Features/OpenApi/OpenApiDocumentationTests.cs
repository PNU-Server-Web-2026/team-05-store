using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace OnlineStore.Api.Tests.Features.OpenApi;

public class OpenApiDocumentationTests
{
    [Fact]
    public async Task Development_OpenApiDocument_HasInfoAndXmlComments()
    {
        await using var factory = new EnvironmentApiFactory("Development");
        var client = factory.CreateClient();

        var response = await client.GetAsync("/openapi/v1.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await using var stream = await response.Content.ReadAsStreamAsync();
        using var document = await JsonDocument.ParseAsync(stream);
        var root = document.RootElement;
        var info = root.GetProperty("info");

        Assert.Equal("Online Store / Warehouse API", info.GetProperty("title").GetString());
        Assert.Equal("v1", info.GetProperty("version").GetString());
        Assert.Contains("склад", info.GetProperty("description").GetString(), StringComparison.OrdinalIgnoreCase);

        var json = root.GetRawText();
        Assert.Contains("pong", json, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Development_ScalarUi_ReturnsHtml()
    {
        await using var factory = new EnvironmentApiFactory("Development");
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = true
        });

        var response = await client.GetAsync("/scalar");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("html", response.Content.Headers.ContentType?.MediaType, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Production_DoesNotPublishOpenApiOrScalar()
    {
        await using var factory = new EnvironmentApiFactory("Production");
        var client = factory.CreateClient();

        var openApi = await client.GetAsync("/openapi/v1.json");
        var scalar = await client.GetAsync("/scalar");

        Assert.Equal(HttpStatusCode.NotFound, openApi.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, scalar.StatusCode);
    }

    private sealed class EnvironmentApiFactory(string environment) : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment(environment);
        }
    }
}
