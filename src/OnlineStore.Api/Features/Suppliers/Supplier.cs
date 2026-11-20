namespace OnlineStore.Api.Features.Suppliers;

/// <summary>
/// Сутність постачальника у PostgreSQL.
/// Унікальний індекс EdrpouCode і зв'язок з Product додаються в окремих задачах.
/// </summary>
public class Supplier
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string EdrpouCode { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Phone { get; set; }
}
