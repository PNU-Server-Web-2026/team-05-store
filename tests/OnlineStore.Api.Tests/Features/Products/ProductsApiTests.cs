using System.Net;
using System.Net.Http.Json;

namespace OnlineStore.Api.Tests.Features.Products;

public class ProductsApiTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetAll_ReturnsOkWithArray()
    {
        var response = await _client.GetAsync("/api/products");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<List<ProductResponse>>();
        Assert.NotNull(body);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenProductExists()
    {
        var created = await CreateAsync();

        var response = await _client.GetAsync($"/api/products/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ProductResponse>();
        Assert.NotNull(body);
        Assert.Equal(created.Id, body.Id);
        Assert.Equal(created.Name, body.Name);
        Assert.Equal(created.Sku, body.Sku);
        Assert.Equal(created.Price, body.Price);
        Assert.Equal(created.Description, body.Description);
        Assert.Equal(created.IsActive, body.IsActive);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMissing()
    {
        var response = await _client.GetAsync($"/api/products/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_ReturnsCreatedWithLocation()
    {
        var request = NewRequest();

        var response = await _client.PostAsJsonAsync("/api/products", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ProductResponse>();
        Assert.NotNull(body);
        Assert.NotEqual(Guid.Empty, body.Id);
        Assert.Equal(request.Name, body.Name);
        Assert.Equal(request.Sku, body.Sku);
        Assert.Equal(request.Price, body.Price);
        Assert.Equal(request.Description, body.Description);
        Assert.Equal(request.IsActive, body.IsActive);
        Assert.NotNull(response.Headers.Location);
        Assert.Equal($"/api/products/{body.Id}", response.Headers.Location.AbsolutePath);

        var followUp = await _client.GetAsync(response.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, followUp.StatusCode);
    }

    [Fact]
    public async Task Put_ReturnsNoContent_WhenProductExists()
    {
        var created = await CreateAsync();
        var request = new UpdateProductRequest("Updated name", created.Sku, 199.99m, "Updated description", false);

        var response = await _client.PutAsJsonAsync($"/api/products/{created.Id}", request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var get = await _client.GetAsync($"/api/products/{created.Id}");
        var body = await get.Content.ReadFromJsonAsync<ProductResponse>();
        Assert.NotNull(body);
        Assert.Equal(request.Name, body.Name);
        Assert.Equal(request.Price, body.Price);
        Assert.Equal(request.Description, body.Description);
        Assert.False(body.IsActive);
    }

    [Fact]
    public async Task Put_ReturnsNotFound_WhenMissing()
    {
        var request = new UpdateProductRequest("Missing", "MISSING-1", 10m, "none", true);

        var response = await _client.PutAsJsonAsync($"/api/products/{Guid.NewGuid()}", request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenProductExists()
    {
        var created = await CreateAsync();

        var response = await _client.DeleteAsync($"/api/products/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var get = await _client.GetAsync($"/api/products/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenMissing()
    {
        var response = await _client.DeleteAsync($"/api/products/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<ProductResponse> CreateAsync()
    {
        var response = await _client.PostAsJsonAsync("/api/products", NewRequest());
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<ProductResponse>();
        Assert.NotNull(body);
        return body;
    }

    private static CreateProductRequest NewRequest()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
        return new CreateProductRequest($"Phone {suffix}", $"PHONE-{suffix}", 499.90m, "Mobile phone", true);
    }

    private sealed record CreateProductRequest(string Name, string Sku, decimal Price, string? Description, bool IsActive);

    private sealed record UpdateProductRequest(string Name, string Sku, decimal Price, string? Description, bool IsActive);

    private sealed record ProductResponse(Guid Id, string Name, string Sku, decimal Price, string? Description, bool IsActive);
}
