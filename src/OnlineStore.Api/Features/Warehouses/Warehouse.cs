namespace OnlineStore.Api.Features.Warehouses;

/// <summary>
/// Склад у PostgreSQL. Унікальний Name і check Priority — задача #29.
/// </summary>
public class Warehouse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public int Priority { get; set; }
}