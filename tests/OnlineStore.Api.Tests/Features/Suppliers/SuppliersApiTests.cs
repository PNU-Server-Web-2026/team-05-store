using System.Net;
using System.Net.Http.Json;
using OnlineStore.Api.Features.Suppliers;
using OnlineStore.Api.Tests.Infrastructure;

namespace OnlineStore.Api.Tests.Features.Suppliers;

public sealed class SuppliersApiTests(ApiFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task GetAll_ReturnsOkWithArray()
    {
        var response = await Client.GetAsync("/api/suppliers");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<List<SupplierResponse>>();
        Assert.NotNull(body);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenSupplierExists()
    {
        var created = await CreateAsync();

        var response = await Client.GetAsync($"/api/suppliers/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<SupplierResponse>();
        Assert.NotNull(body);
        Assert.Equal(created.Id, body.Id);
        Assert.Equal(created.Name, body.Name);
        Assert.Equal(created.EdrpouCode, body.EdrpouCode);
        Assert.Equal(created.Email, body.Email);
        Assert.Equal(created.Phone, body.Phone);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMissing()
    {
        var response = await Client.GetAsync($"/api/suppliers/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_ReturnsCreatedWithLocation()
    {
        var request = NewRequest();

        var response = await Client.PostAsJsonAsync("/api/suppliers", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<SupplierResponse>();
        Assert.NotNull(body);
        Assert.NotEqual(Guid.Empty, body.Id);
        Assert.Equal(request.Name, body.Name);
        Assert.Equal(request.EdrpouCode, body.EdrpouCode);
        Assert.Equal(request.Email, body.Email);
        Assert.Equal(request.Phone, body.Phone);
        Assert.NotNull(response.Headers.Location);
        Assert.Equal($"/api/suppliers/{body.Id}", response.Headers.Location.AbsolutePath);

        var followUp = await Client.GetAsync(response.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, followUp.StatusCode);
    }

    [Fact]
    public async Task Put_ReturnsNoContent_WhenSupplierExists()
    {
        var created = await CreateAsync();
        var request = new UpdateSupplierRequest(
            "Updated supplier",
            created.EdrpouCode,
            "updated@example.com",
            "+380501112233");

        var response = await Client.PutAsJsonAsync($"/api/suppliers/{created.Id}", request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var get = await Client.GetAsync($"/api/suppliers/{created.Id}");
        var body = await get.Content.ReadFromJsonAsync<SupplierResponse>();
        Assert.NotNull(body);
        Assert.Equal(request.Name, body.Name);
        Assert.Equal(request.Email, body.Email);
        Assert.Equal(request.Phone, body.Phone);
    }

    [Fact]
    public async Task Put_ReturnsNotFound_WhenMissing()
    {
        var request = new UpdateSupplierRequest("Missing", "12345678", "missing@example.com", "+380501234567");

        var response = await Client.PutAsJsonAsync($"/api/suppliers/{Guid.NewGuid()}", request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Put_ConcurrentUpdates_WithoutDeletion_AllReturnNoContent()
    {
        var created = await CreateAsync();
        const int concurrency = 20;

        var tasks = Enumerable.Range(0, concurrency).Select(async i =>
        {
            var request = new UpdateSupplierRequest(
                $"Concurrent supplier {i}",
                created.EdrpouCode,
                $"concurrent-{i}@example.com",
                $"+38050{i:D7}");
            return await Client.PutAsJsonAsync($"/api/suppliers/{created.Id}", request);
        });

        var responses = await Task.WhenAll(tasks);

        Assert.All(responses, response => Assert.Equal(HttpStatusCode.NoContent, response.StatusCode));
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenSupplierExists()
    {
        var created = await CreateAsync();

        var response = await Client.DeleteAsync($"/api/suppliers/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var get = await Client.GetAsync($"/api/suppliers/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenMissing()
    {
        var response = await Client.DeleteAsync($"/api/suppliers/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<SupplierResponse> CreateAsync()
    {
        var response = await Client.PostAsJsonAsync("/api/suppliers", NewRequest());
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<SupplierResponse>();
        Assert.NotNull(body);
        return body;
    }

    private static CreateSupplierRequest NewRequest()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var edrpou = Random.Shared.Next(10_000_000, 99_999_999).ToString();
        return new CreateSupplierRequest(
            $"Supplier {suffix}",
            edrpou,
            $"supplier-{suffix}@example.com",
            "+380671112233");
    }
}
