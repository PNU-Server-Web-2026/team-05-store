using System.Net;
using System.Net.Http.Json;
using OnlineStore.Api.Features.Products;
using OnlineStore.Api.Tests.Infrastructure;

namespace OnlineStore.Api.Tests.Features.Products;

public sealed class ProductsApiTests(ApiFactory factory) : IntegrationTestBase(factory)
{

    [Fact]
    public async Task GetAll_ReturnsOkWithArray()
    {
        var created = await CreateAsync();

        var response = await Client.GetAsync("/api/products");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<List<ProductResponse>>();
        Assert.NotNull(body);
        Assert.Contains(body, product => product.Id == created.Id);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenProductExists()
    {
        var created = await CreateAsync();

        var response = await Client.GetAsync($"/api/products/{created.Id}");

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
        var response = await Client.GetAsync($"/api/products/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_ReturnsCreatedWithLocation()
    {
        var request = NewRequest();

        var response = await Client.PostAsJsonAsync("/api/products", request);

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

        var followUp = await Client.GetAsync(response.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, followUp.StatusCode);
    }

    [Fact]
    public async Task Put_ReturnsNoContent_WhenProductExists()
    {
        var created = await CreateAsync();
        var request = new UpdateProductRequest
        {
            Name = "Updated name",
            Sku = created.Sku,
            Price = 199.99m,
            Description = "Updated description",
            IsActive = false
        };

        var response = await Client.PutAsJsonAsync($"/api/products/{created.Id}", request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var get = await Client.GetAsync($"/api/products/{created.Id}");
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
        var request = new UpdateProductRequest
        {
            Name = "Missing",
            Sku = "MISSING-1",
            Price = 10m,
            Description = "none",
            IsActive = true
        };

        var response = await Client.PutAsJsonAsync($"/api/products/{Guid.NewGuid()}", request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenProductExists()
    {
        var created = await CreateAsync();

        var response = await Client.DeleteAsync($"/api/products/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var get = await Client.GetAsync($"/api/products/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenMissing()
    {
        var response = await Client.DeleteAsync($"/api/products/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<ProductResponse> CreateAsync()
    {
        var response = await Client.PostAsJsonAsync("/api/products", NewRequest());
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<ProductResponse>();
        Assert.NotNull(body);
        return body;
    }

    private static CreateProductRequest NewRequest()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
        return new CreateProductRequest
        {
            Name = $"Phone {suffix}",
            Sku = $"PHONE-{suffix}",
            Price = 499.90m,
            Description = "Mobile phone",
            IsActive = true
        };
    }
}
