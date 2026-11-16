using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
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
    public async Task Post_WhenSkuMissing_ReturnsValidationProblemForSku()
    {
        var request = new CreateProductRequest { Name = "Phone", Sku = "", Price = 10m, IsActive = true };

        var response = await Client.PostAsJsonAsync("/api/products", request);

        await ValidationProblemAssert.AssertHasFieldErrorAsync(response, "Sku");
    }

    [Fact]
    public async Task Post_WhenNamePropertyOmitted_ReturnsValidationProblemForName()
    {
        var response = await PostJsonAsync("""{"sku":"PHONE-1","price":10,"isActive":true}""");

        await ValidationProblemAssert.AssertHasFieldErrorAsync(response, "Name");
    }

    [Fact]
    public async Task Post_WhenSkuPropertyOmitted_ReturnsValidationProblemForSku()
    {
        var response = await PostJsonAsync("""{"name":"Phone","price":10,"isActive":true}""");

        await ValidationProblemAssert.AssertHasFieldErrorAsync(response, "Sku");
    }

    [Theory]
    [InlineData("PHONE--1")]
    [InlineData("PHONE-")]
    public async Task Post_WhenSkuAllowsHyphenSegments_ReturnsCreated(string sku)
    {
        var suffix = Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
        var request = new CreateProductRequest
        {
            Name = $"Phone {suffix}",
            Sku = sku,
            Price = 19.99m,
            IsActive = true
        };

        var response = await Client.PostAsJsonAsync("/api/products", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Post_WithUkCultureAndValidPrice_ReturnsCreated()
    {
        await using var ukFactory = Factory.WithWebHostBuilder(builder =>
            builder.ConfigureTestServices(services =>
                services.AddSingleton<IStartupFilter, UkCultureStartupFilter>()));

        var client = ukFactory.CreateClient();
        var suffix = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
        var request = new CreateProductRequest
        {
            Name = $"Phone {suffix}",
            Sku = $"PHONE-{suffix}",
            Price = 10.50m,
            IsActive = true
        };

        var response = await client.PostAsJsonAsync("/api/products", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Put_WhenNamePropertyOmitted_ReturnsValidationProblemForName()
    {
        var created = await CreateAsync();
        var response = await PutJsonAsync(
            created.Id,
            $$"""{"sku":"{{created.Sku}}","price":10,"isActive":true}""");

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

    private sealed class UkCultureStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) =>
            app =>
            {
                app.Use(async (context, nextMiddleware) =>
                {
                    var culture = new CultureInfo("uk-UA");
                    CultureInfo.CurrentCulture = culture;
                    CultureInfo.CurrentUICulture = culture;
                    await nextMiddleware();
                });
                next(app);
            };
    }

    private Task<HttpResponseMessage> PostJsonAsync(string json) =>
        Client.PostAsync(
            "/api/products",
            new StringContent(json, Encoding.UTF8, "application/json"));

    private Task<HttpResponseMessage> PutJsonAsync(Guid id, string json) =>
        Client.PutAsync(
            $"/api/products/{id}",
            new StringContent(json, Encoding.UTF8, "application/json"));

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
