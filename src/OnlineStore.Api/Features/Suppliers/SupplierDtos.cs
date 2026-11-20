namespace OnlineStore.Api.Features.Suppliers;

public sealed record CreateSupplierRequest(string Name, string EdrpouCode, string? Email, string? Phone);

public sealed record UpdateSupplierRequest(string Name, string EdrpouCode, string? Email, string? Phone);

public sealed record SupplierResponse(Guid Id, string Name, string EdrpouCode, string? Email, string? Phone);
