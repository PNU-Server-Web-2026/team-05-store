namespace OnlineStore.Api.Features.Categories;

public sealed record CreateCategoryRequest(string Name, string Slug, string? Description);

public sealed record UpdateCategoryRequest(string Name, string Slug, string? Description);

public sealed record CategoryResponse(Guid Id, string Name, string Slug, string? Description);
