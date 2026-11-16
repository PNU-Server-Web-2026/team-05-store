using System.ComponentModel.DataAnnotations;
using OnlineStore.Api.Common.Validation;

namespace OnlineStore.Api.Features.Products;

public sealed record CreateProductRequest
{
    [Required(AllowEmptyStrings = false)]
    [MaxLength(200)]
    public string Name { get; init; } = default!;

    [Required(AllowEmptyStrings = false)]
    [MaxLength(30)]
    [RegularExpression(@"^[A-Z0-9-]+$", ErrorMessage = "Sku must contain only uppercase letters, digits, and hyphens.")]
    public string Sku { get; init; } = default!;

    [Range(typeof(decimal), "0.01", "1000000", ParseLimitsInInvariantCulture = true)]
    [MaxDecimalPlaces(2)]
    public decimal Price { get; init; }

    [MaxLength(4000)]
    public string? Description { get; init; }

    public bool IsActive { get; init; }
}

public sealed record UpdateProductRequest
{
    [Required(AllowEmptyStrings = false)]
    [MaxLength(200)]
    public string Name { get; init; } = default!;

    [Required(AllowEmptyStrings = false)]
    [MaxLength(30)]
    [RegularExpression(@"^[A-Z0-9-]+$", ErrorMessage = "Sku must contain only uppercase letters, digits, and hyphens.")]
    public string Sku { get; init; } = default!;

    [Range(typeof(decimal), "0.01", "1000000", ParseLimitsInInvariantCulture = true)]
    [MaxDecimalPlaces(2)]
    public decimal Price { get; init; }

    [MaxLength(4000)]
    public string? Description { get; init; }

    public bool IsActive { get; init; }
}

public sealed record ProductResponse(Guid Id, string Name, string Sku, decimal Price, string? Description, bool IsActive);