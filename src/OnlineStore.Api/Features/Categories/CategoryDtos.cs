using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Api.Features.Categories;

public sealed class CreateCategoryRequest
{
    [Required(AllowEmptyStrings = false)]
    [MaxLength(100)]
    public required string Name { get; init; }

    [Required(AllowEmptyStrings = false)]
    [MaxLength(100)]
    [RegularExpression(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", ErrorMessage = "Slug must contain only lowercase letters, digits, and hyphens.")]
    public required string Slug { get; init; }

    [MaxLength(1000)]
    public string? Description { get; init; }
}

public sealed class UpdateCategoryRequest
{
    [Required(AllowEmptyStrings = false)]
    [MaxLength(100)]
    public required string Name { get; init; }

    [Required(AllowEmptyStrings = false)]
    [MaxLength(100)]
    [RegularExpression(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", ErrorMessage = "Slug must contain only lowercase letters, digits, and hyphens.")]
    public required string Slug { get; init; }

    [MaxLength(1000)]
    public string? Description { get; init; }
}

public sealed record CategoryResponse(Guid Id, string Name, string Slug, string? Description);
