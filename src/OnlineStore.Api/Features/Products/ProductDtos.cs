using System.ComponentModel.DataAnnotations;
using OnlineStore.Api.Common.Validation;

namespace OnlineStore.Api.Features.Products;

public sealed class CreateProductRequest
{
    [Required(AllowEmptyStrings = false)]
    [MaxLength(200)]
    public required string Name { get; init; }

    [Required(AllowEmptyStrings = false)]
    [MaxLength(30)]
    [RegularExpression(@"^[A-Z0-9]+(?:-[A-Z0-9]+)*$", ErrorMessage = "Sku must contain only uppercase letters, digits, and hyphens.")]
    public required string Sku { get; init; }

    [Range(typeof(decimal), "0.01", "1000000")]
    [MaxDecimalPlaces(2)]
    public decimal Price { get; init; }

    [MaxLength(4000)]
    public string? Description { get; init; }

    public bool IsActive { get; init; }
}

public sealed class UpdateProductRequest
{
    [Required(AllowEmptyStrings = false)]
    [MaxLength(200)]
    public required string Name { get; init; }

    [Required(AllowEmptyStrings = false)]
    [MaxLength(30)]
    [RegularExpression(@"^[A-Z0-9]+(?:-[A-Z0-9]+)*$", ErrorMessage = "Sku must contain only uppercase letters, digits, and hyphens.")]
    public required string Sku { get; init; }

    [Range(typeof(decimal), "0.01", "1000000")]
    [MaxDecimalPlaces(2)]
    public decimal Price { get; init; }

    [MaxLength(4000)]
    public string? Description { get; init; }

    public bool IsActive { get; init; }
}

public sealed record ProductResponse(Guid Id, string Name, string Sku, decimal Price, string? Description, bool IsActive);
