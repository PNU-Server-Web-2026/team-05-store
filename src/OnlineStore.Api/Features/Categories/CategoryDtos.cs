namespace OnlineStore.Api.Features.Categories;

public sealed record CreateCategoryRequest(string Name, string Slug, string Description);

public sealed record UpdateCategoryRequest(string Name, string Slug, string Description);

public sealed record CategoryResponse(int Id, string Name, string Slug, string Description);
