using System.ComponentModel.DataAnnotations;
using OnlineStore.Api.Common.Validation;

namespace OnlineStore.Api.Features.Products;

public sealed record CreateProductRequest(
    [property: Required(AllowEmptyStrings = false)]
    [property: MaxLength(200)]
    string Name,
    [property: Required(AllowEmptyStrings = false)]
    [property: MaxLength(30)]
    [property: RegularExpression(@"^[A-Z0-9]+(?:-[A-Z0-9]+)*$", ErrorMessage = "Sku must contain only uppercase letters, digits, and hyphens.")]
    string Sku,
    [property: Range(typeof(decimal), "0.01", "1000000")]
    [property: MaxDecimalPlaces(2)]
    decimal Price,
    [property: MaxLength(4000)]
    string? Description,
    bool IsActive);

public sealed record UpdateProductRequest(
    [property: Required(AllowEmptyStrings = false)]
    [property: MaxLength(200)]
    string Name,
    [property: Required(AllowEmptyStrings = false)]
    [property: MaxLength(30)]
    [property: RegularExpression(@"^[A-Z0-9]+(?:-[A-Z0-9]+)*$", ErrorMessage = "Sku must contain only uppercase letters, digits, and hyphens.")]
    string Sku,
    [property: Range(typeof(decimal), "0.01", "1000000")]
    [property: MaxDecimalPlaces(2)]
    decimal Price,
    [property: MaxLength(4000)]
    string? Description,
    bool IsActive);

public sealed record ProductResponse(Guid Id, string Name, string Sku, decimal Price, string? Description, bool IsActive);
