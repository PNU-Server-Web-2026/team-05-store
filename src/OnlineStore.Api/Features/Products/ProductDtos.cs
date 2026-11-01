namespace OnlineStore.Api.Features.Products;

public sealed record CreateProductRequest(string Name, string Sku, decimal Price, string? Description, bool IsActive);

public sealed record UpdateProductRequest(string Name, string Sku, decimal Price, string? Description, bool IsActive);

public sealed record ProductResponse(Guid Id, string Name, string Sku, decimal Price, string? Description, bool IsActive);
