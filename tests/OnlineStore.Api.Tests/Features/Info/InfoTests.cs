using System.Net;
using System.Net.Http.Json;
using OnlineStore.Api.Common;
using OnlineStore.Api.Features.Info;
using OnlineStore.Api.Tests.Infrastructure;

namespace OnlineStore.Api.Tests.Features.Info;

public sealed class InfoTests(ApiFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task Get_ReturnsApiInfo()
    {
        var response = await Client.GetAsync("/api/info");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<InfoResponse>();
        Assert.NotNull(body);
        Assert.Equal(ApiInfo.Name, body.Name);
        Assert.False(string.IsNullOrWhiteSpace(body.Version));
        Assert.Equal("Testing", body.Environment);
        Assert.InRange(body.UtcNow, DateTime.UtcNow.AddMinutes(-1), DateTime.UtcNow.AddMinutes(1));
    }
}
