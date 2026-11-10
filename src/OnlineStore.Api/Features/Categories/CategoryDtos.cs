using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Api.Features.Categories;

public sealed record CreateCategoryRequest(
    [property: Required(AllowEmptyStrings = false)]
    [property: MaxLength(100)]
    string Name,
    [property: Required(AllowEmptyStrings = false)]
    [property: MaxLength(100)]
    [property: RegularExpression(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", ErrorMessage = "Slug must contain only lowercase letters, digits, and hyphens.")]
    string Slug,
    [property: MaxLength(1000)]
    string? Description);

public sealed record UpdateCategoryRequest(
    [property: Required(AllowEmptyStrings = false)]
    [property: MaxLength(100)]
    string Name,
    [property: Required(AllowEmptyStrings = false)]
    [property: MaxLength(100)]
    [property: RegularExpression(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", ErrorMessage = "Slug must contain only lowercase letters, digits, and hyphens.")]
    string Slug,
    [property: MaxLength(1000)]
    string? Description);

public sealed record CategoryResponse(Guid Id, string Name, string Slug, string? Description);
