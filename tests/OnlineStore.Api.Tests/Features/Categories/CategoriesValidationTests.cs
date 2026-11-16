using System.Net.Http.Json;
using System.Text;
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
    public async Task Post_WhenNamePropertyOmitted_ReturnsValidationProblemForName()
    {
        var response = await PostJsonAsync("""{"slug":"phones","description":"d"}""");

        await ValidationProblemAssert.AssertHasFieldErrorAsync(response, "Name");
    }

    [Fact]
    public async Task Post_WhenSlugPropertyOmitted_ReturnsValidationProblemForSlug()
    {
        var response = await PostJsonAsync("""{"name":"Phones","description":"d"}""");

        await ValidationProblemAssert.AssertHasFieldErrorAsync(response, "Slug");
    }

    [Fact]
    public async Task Post_WhenNameExceedsMaxLength_ReturnsValidationProblemForName()
    {
        var name = new string('n', 101);
        var request = new CreateCategoryRequest { Name = name, Slug = "valid-slug" };

        var response = await Client.PostAsJsonAsync("/api/categories", request);

        await ValidationProblemAssert.AssertHasFieldErrorAsync(response, "Name");
    }

    [Fact]
    public async Task Post_WhenDescriptionExceedsMaxLength_ReturnsValidationProblemForDescription()
    {
        var description = new string('d', 1001);
        var request = new CreateCategoryRequest { Name = "Phones", Slug = "phones", Description = description };

        var response = await Client.PostAsJsonAsync("/api/categories", request);

        await ValidationProblemAssert.AssertHasFieldErrorAsync(response, "Description");
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
    public async Task Put_WhenNamePropertyOmitted_ReturnsValidationProblemForName()
    {
        var created = await CreateAsync();

        var response = await PutJsonAsync(
            created.Id,
            $$"""{"slug":"{{created.Slug}}","description":"d"}""");

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

    private Task<HttpResponseMessage> PostJsonAsync(string json) =>
        Client.PostAsync(
            "/api/categories",
            new StringContent(json, Encoding.UTF8, "application/json"));

    private Task<HttpResponseMessage> PutJsonAsync(Guid id, string json) =>
        Client.PutAsync(
            $"/api/categories/{id}",
            new StringContent(json, Encoding.UTF8, "application/json"));

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
