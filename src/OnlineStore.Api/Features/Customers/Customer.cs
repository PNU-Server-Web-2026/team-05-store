namespace OnlineStore.Api.Features.Customers;

/// <summary>
/// Покупець у PostgreSQL. Унікальний Email — задача #29; зв'язок з User — #66.
/// </summary>
public class Customer
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string ShippingAddress { get; set; } = string.Empty;
}