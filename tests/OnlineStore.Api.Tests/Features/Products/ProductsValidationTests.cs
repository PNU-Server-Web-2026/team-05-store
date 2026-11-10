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
        var request = new CreateProductRequest("", "SKU-1", 10m, null, true);

        var response = await Client.PostAsJsonAsync("/api/products", request);

        await ValidationProblemAssert.AssertHasFieldErrorAsync(response, "Name");
    }

    [Fact]
    public async Task Post_WhenSkuHasLowercase_ReturnsValidationProblemForSku()
    {
        var request = new CreateProductRequest("Phone", "phone-1", 10m, null, true);

        var response = await Client.PostAsJsonAsync("/api/products", request);

        await ValidationProblemAssert.AssertHasFieldErrorAsync(response, "Sku");
    }

    [Fact]
    public async Task Post_WhenPriceIsZero_ReturnsValidationProblemForPrice()
    {
        var request = new CreateProductRequest("Phone", "PHONE-1", 0m, null, true);

        var response = await Client.PostAsJsonAsync("/api/products", request);

        await ValidationProblemAssert.AssertHasFieldErrorAsync(response, "Price");
    }

    [Fact]
    public async Task Post_WhenPriceExceedsMaximum_ReturnsValidationProblemForPrice()
    {
        var request = new CreateProductRequest("Phone", "PHONE-1", 1_000_000.01m, null, true);

        var response = await Client.PostAsJsonAsync("/api/products", request);

        await ValidationProblemAssert.AssertHasFieldErrorAsync(response, "Price");
    }

    [Fact]
    public async Task Post_WhenPriceHasTooManyDecimalPlaces_ReturnsValidationProblemForPrice()
    {
        var request = new CreateProductRequest("Phone", "PHONE-1", 9.999m, null, true);

        var response = await Client.PostAsJsonAsync("/api/products", request);

        await ValidationProblemAssert.AssertHasFieldErrorAsync(response, "Price");
    }

    [Fact]
    public async Task Put_WhenSkuInvalid_ReturnsValidationProblemForSku()
    {
        var created = await CreateAsync();
        var request = new UpdateProductRequest(created.Name, "bad_sku", created.Price, created.Description, created.IsActive);

        var response = await Client.PutAsJsonAsync($"/api/products/{created.Id}", request);

        await ValidationProblemAssert.AssertHasFieldErrorAsync(response, "Sku");
    }

    private async Task<ProductResponse> CreateAsync()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
        var response = await Client.PostAsJsonAsync(
            "/api/products",
            new CreateProductRequest($"Phone {suffix}", $"PHONE-{suffix}", 99.99m, null, true));
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<ProductResponse>();
        Assert.NotNull(body);
        return body;
    }
}
