using System.Net;
using OnlineStore.Api.Common;
using OnlineStore.Api.Tests.Infrastructure;

namespace OnlineStore.Api.Tests.Common;

public sealed class CorrelationIdApiTests(ApiFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task Get_Ping_WithoutHeader_ReturnsGeneratedCorrelationId()
    {
        var response = await Client.GetAsync("/api/ping");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.True(response.Headers.TryGetValues(CorrelationIdMiddleware.HeaderName, out var values));
        var correlationId = Assert.Single(values);
        Assert.True(Guid.TryParse(correlationId, out _));
    }

    [Fact]
    public async Task Get_Ping_WithHeader_ReturnsSameCorrelationId()
    {
        const string incoming = "pipeline-correlation-id";
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/ping");
        request.Headers.TryAddWithoutValidation(CorrelationIdMiddleware.HeaderName, incoming);

        var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.TryGetValues(CorrelationIdMiddleware.HeaderName, out var values));
        Assert.Equal(incoming, Assert.Single(values));
    }
}
