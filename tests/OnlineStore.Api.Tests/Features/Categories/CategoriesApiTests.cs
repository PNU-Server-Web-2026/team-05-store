using System.Net;
using System.Net.Http.Json;

namespace OnlineStore.Api.Tests.Features.Categories;

public class CategoriesApiTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetAll_ReturnsOkWithArray()
    {
        var response = await _client.GetAsync("/api/categories");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<List<CategoryResponse>>();
        Assert.NotNull(body);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenCategoryExists()
    {
        var created = await CreateAsync();

        var response = await _client.GetAsync($"/api/categories/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<CategoryResponse>();
        Assert.NotNull(body);
        Assert.Equal(created.Id, body.Id);
        Assert.Equal(created.Name, body.Name);
        Assert.Equal(created.Slug, body.Slug);
        Assert.Equal(created.Description, body.Description);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMissing()
    {
        var response = await _client.GetAsync("/api/categories/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_ReturnsCreatedWithLocation()
    {
        var request = NewRequest();

        var response = await _client.PostAsJsonAsync("/api/categories", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<CategoryResponse>();
        Assert.NotNull(body);
        Assert.True(body.Id > 0);
        Assert.Equal(request.Name, body.Name);
        Assert.Equal(request.Slug, body.Slug);
        Assert.Equal(request.Description, body.Description);
        Assert.NotNull(response.Headers.Location);
        Assert.Equal($"/api/categories/{body.Id}", response.Headers.Location.AbsolutePath);

        var followUp = await _client.GetAsync(response.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, followUp.StatusCode);
    }

    [Fact]
    public async Task Put_ReturnsNoContent_WhenCategoryExists()
    {
        var created = await CreateAsync();
        var request = new UpdateCategoryRequest("Updated name", created.Slug, "Updated description");

        var response = await _client.PutAsJsonAsync($"/api/categories/{created.Id}", request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var get = await _client.GetAsync($"/api/categories/{created.Id}");
        var body = await get.Content.ReadFromJsonAsync<CategoryResponse>();
        Assert.NotNull(body);
        Assert.Equal(request.Name, body.Name);
        Assert.Equal(request.Description, body.Description);
    }

    [Fact]
    public async Task Put_ReturnsNotFound_WhenMissing()
    {
        var request = new UpdateCategoryRequest("Missing", "missing", "none");

        var response = await _client.PutAsJsonAsync("/api/categories/999999", request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenCategoryExists()
    {
        var created = await CreateAsync();

        var response = await _client.DeleteAsync($"/api/categories/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var get = await _client.GetAsync($"/api/categories/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenMissing()
    {
        var response = await _client.DeleteAsync("/api/categories/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<CategoryResponse> CreateAsync()
    {
        var response = await _client.PostAsJsonAsync("/api/categories", NewRequest());
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<CategoryResponse>();
        Assert.NotNull(body);
        return body;
    }

    private static CreateCategoryRequest NewRequest()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        return new CreateCategoryRequest($"Phones {suffix}", $"smart-phones-{suffix}", "Mobile phones");
    }

    private sealed record CreateCategoryRequest(string Name, string Slug, string Description);

    private sealed record UpdateCategoryRequest(string Name, string Slug, string Description);

    private sealed record CategoryResponse(int Id, string Name, string Slug, string Description);
}
