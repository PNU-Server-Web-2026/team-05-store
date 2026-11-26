namespace OnlineStore.Api.Features.Products;

/// <summary>
/// Сутність товару в PostgreSQL.
/// Унікальний Sku і check Price більше 0 — задача #29; CategoryId — #30; SupplierId — #31.
/// </summary>
public class Product
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Sku { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; }
}
