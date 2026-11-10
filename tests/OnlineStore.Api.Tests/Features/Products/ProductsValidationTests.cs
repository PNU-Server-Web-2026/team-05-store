using System.Net.Http.Json;
using OnlineStore.Api.Features.Products;
using OnlineStore.Api.Tests.Common;
using OnlineStore.Api.Tests.Infrastructure;

namespace OnlineStore.Api.Tests.Features.Products;

public sealed class ProductsValidationTests(ApiFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task Post_WhenNameMissing_ReturnsValidationProblemForName()
    {
        var request = new CreateProductRequest { Name = "", Sku = "SKU-1", Price = 10m, IsActive = true };

        var response = await Client.PostAsJsonAsync("/api/products", request);

        await ValidationProblemAssert.AssertHasFieldErrorAsync(response, "Name");
    }

    [Fact]
    public async Task Post_WhenSkuHasLowercase_ReturnsValidationProblemForSku()
    {
        var request = new CreateProductRequest { Name = "Phone", Sku = "phone-1", Price = 10m, IsActive = true };

        var response = await Client.PostAsJsonAsync("/api/products", request);

        await ValidationProblemAssert.AssertHasFieldErrorAsync(response, "Sku");
    }

    [Fact]
    public async Task Post_WhenPriceIsZero_ReturnsValidationProblemForPrice()
    {
        var request = new CreateProductRequest { Name = "Phone", Sku = "PHONE-1", Price = 0m, IsActive = true };

        var response = await Client.PostAsJsonAsync("/api/products", request);

        await ValidationProblemAssert.AssertHasFieldErrorAsync(response, "Price");
    }

    [Fact]
    public async Task Post_WhenPriceExceedsMaximum_ReturnsValidationProblemForPrice()
    {
        var request = new CreateProductRequest { Name = "Phone", Sku = "PHONE-1", Price = 1_000_000.01m, IsActive = true };

        var response = await Client.PostAsJsonAsync("/api/products", request);

        await ValidationProblemAssert.AssertHasFieldErrorAsync(response, "Price");
    }

    [Fact]
    public async Task Post_WhenPriceHasTooManyDecimalPlaces_ReturnsValidationProblemForPrice()
    {
        var request = new CreateProductRequest { Name = "Phone", Sku = "PHONE-1", Price = 9.999m, IsActive = true };

        var response = await Client.PostAsJsonAsync("/api/products", request);

        await ValidationProblemAssert.AssertHasFieldErrorAsync(response, "Price");
    }

    [Fact]
    public async Task Put_WhenSkuInvalid_ReturnsValidationProblemForSku()
    {
        var created = await CreateAsync();
        var request = new UpdateProductRequest
        {
            Name = created.Name,
            Sku = "bad_sku",
            Price = created.Price,
            Description = created.Description,
            IsActive = created.IsActive
        };

        var response = await Client.PutAsJsonAsync($"/api/products/{created.Id}", request);

        await ValidationProblemAssert.AssertHasFieldErrorAsync(response, "Sku");
    }

    private async Task<ProductResponse> CreateAsync()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
        var response = await Client.PostAsJsonAsync(
            "/api/products",
            new CreateProductRequest
            {
                Name = $"Phone {suffix}",
                Sku = $"PHONE-{suffix}",
                Price = 99.99m,
                IsActive = true
            });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<ProductResponse>();
        Assert.NotNull(body);
        return body;
    }
}
