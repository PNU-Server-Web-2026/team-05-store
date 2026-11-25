using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Api.Features.Categories;

public sealed record CreateCategoryRequest
{
    [Required(AllowEmptyStrings = false)]
    [MaxLength(100)]
    public string Name { get; init; } = default!;

    [Required(AllowEmptyStrings = false)]
    [MaxLength(100)]
    [RegularExpression(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", ErrorMessage = "Slug must contain only lowercase letters, digits, and hyphens.")]
    public string Slug { get; init; } = default!;

    [MaxLength(1000)]
    public string? Description { get; init; }
}

public sealed record UpdateCategoryRequest
{
    [Required(AllowEmptyStrings = false)]
    [MaxLength(100)]
    public string Name { get; init; } = default!;

    [Required(AllowEmptyStrings = false)]
    [MaxLength(100)]
    [RegularExpression(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", ErrorMessage = "Slug must contain only lowercase letters, digits, and hyphens.")]
    public string Slug { get; init; } = default!;

    [MaxLength(1000)]
    public string? Description { get; init; }
}

public sealed record CategoryResponse(Guid Id, string Name, string Slug, string? Description);