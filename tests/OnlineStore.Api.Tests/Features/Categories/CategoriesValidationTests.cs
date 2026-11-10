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
        var request = new CreateCategoryRequest { Name = "", Slug = "valid-slug", Description = "desc" };

        var response = await Client.PostAsJsonAsync("/api/categories", request);

        await ValidationProblemAssert.AssertHasFieldErrorAsync(response, "Name");
    }

    [Fact]
    public async Task Post_WhenSlugMissing_ReturnsValidationProblemForSlug()
    {
        var request = new CreateCategoryRequest { Name = "Phones", Slug = "" };

        var response = await Client.PostAsJsonAsync("/api/categories", request);

        await ValidationProblemAssert.AssertHasFieldErrorAsync(response, "Slug");
    }

    [Fact]
    public async Task Post_WhenSlugHasInvalidCharacters_ReturnsValidationProblemForSlug()
    {
        var request = new CreateCategoryRequest { Name = "Phones", Slug = "INVALID_slug" };

        var response = await Client.PostAsJsonAsync("/api/categories", request);

        await ValidationProblemAssert.AssertHasFieldErrorAsync(response, "Slug");
    }

    [Fact]
    public async Task Post_WhenSlugExceedsMaxLength_ReturnsValidationProblemForSlug()
    {
        var slug = new string('a', 101);
        var request = new CreateCategoryRequest { Name = "Phones", Slug = slug };

        var response = await Client.PostAsJsonAsync("/api/categories", request);

        await ValidationProblemAssert.AssertHasFieldErrorAsync(response, "Slug");
    }

    [Fact]
    public async Task Put_WhenNameMissing_ReturnsValidationProblemForName()
    {
        var created = await CreateAsync();
        var request = new UpdateCategoryRequest
        {
            Name = "",
            Slug = created.Slug,
            Description = created.Description
        };

        var response = await Client.PutAsJsonAsync($"/api/categories/{created.Id}", request);

        await ValidationProblemAssert.AssertHasFieldErrorAsync(response, "Name");
    }

    [Fact]
    public async Task Put_WhenSlugInvalid_ReturnsValidationProblemForSlug()
    {
        var created = await CreateAsync();
        var request = new UpdateCategoryRequest
        {
            Name = created.Name,
            Slug = "bad slug",
            Description = created.Description
        };

        var response = await Client.PutAsJsonAsync($"/api/categories/{created.Id}", request);

        await ValidationProblemAssert.AssertHasFieldErrorAsync(response, "Slug");
    }

    private async Task<CategoryResponse> CreateAsync()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var response = await Client.PostAsJsonAsync(
            "/api/categories",
            new CreateCategoryRequest
            {
                Name = $"Name {suffix}",
                Slug = $"slug-{suffix}",
                Description = "desc"
            });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<CategoryResponse>();
        Assert.NotNull(body);
        return body;
    }
}
