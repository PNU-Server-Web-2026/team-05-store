using System.Net.Http.Json;
using OnlineStore.Api.Features.Categories;
using OnlineStore.Api.Tests.Common;
using OnlineStore.Api.Tests.Infrastructure;

namespace OnlineStore.Api.Tests.Features.Categories;

public sealed class CategoriesValidationTests(ApiFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task Post_WhenNameMissing_ReturnsValidationProblemForName()
    {
        var request = new CreateCategoryRequest("", "valid-slug", "desc");

        var response = await Client.PostAsJsonAsync("/api/categories", request);

        await ValidationProblemAssert.AssertHasFieldErrorAsync(response, "Name");
    }

    [Fact]
    public async Task Post_WhenSlugHasInvalidCharacters_ReturnsValidationProblemForSlug()
    {
        var request = new CreateCategoryRequest("Phones", "INVALID_slug", null);

        var response = await Client.PostAsJsonAsync("/api/categories", request);

        await ValidationProblemAssert.AssertHasFieldErrorAsync(response, "Slug");
    }

    [Fact]
    public async Task Post_WhenSlugExceedsMaxLength_ReturnsValidationProblemForSlug()
    {
        var slug = new string('a', 101);
        var request = new CreateCategoryRequest("Phones", slug, null);

        var response = await Client.PostAsJsonAsync("/api/categories", request);

        await ValidationProblemAssert.AssertHasFieldErrorAsync(response, "Slug");
    }

    [Fact]
    public async Task Put_WhenNameMissing_ReturnsValidationProblemForName()
    {
        var created = await CreateAsync();
        var request = new UpdateCategoryRequest("", created.Slug, created.Description);

        var response = await Client.PutAsJsonAsync($"/api/categories/{created.Id}", request);

        await ValidationProblemAssert.AssertHasFieldErrorAsync(response, "Name");
    }

    [Fact]
    public async Task Put_WhenSlugInvalid_ReturnsValidationProblemForSlug()
    {
        var created = await CreateAsync();
        var request = new UpdateCategoryRequest(created.Name, "bad slug", created.Description);

        var response = await Client.PutAsJsonAsync($"/api/categories/{created.Id}", request);

        await ValidationProblemAssert.AssertHasFieldErrorAsync(response, "Slug");
    }

    private async Task<CategoryResponse> CreateAsync()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var response = await Client.PostAsJsonAsync(
            "/api/categories",
            new CreateCategoryRequest($"Name {suffix}", $"slug-{suffix}", "desc"));
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<CategoryResponse>();
        Assert.NotNull(body);
        return body;
    }
}
