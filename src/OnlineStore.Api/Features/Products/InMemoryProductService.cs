using System.Collections.Concurrent;

namespace OnlineStore.Api.Features.Products;

public sealed class InMemoryProductService : IProductService
{
    private readonly ConcurrentDictionary<Guid, Product> _items = new();

    public Task<IReadOnlyList<ProductResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<ProductResponse> result = _items.Values
            .OrderBy(product => product.Id)
            .Select(ToResponse)
            .ToArray();
        return Task.FromResult(result);
    }

    public Task<ProductResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!_items.TryGetValue(id, out var product))
        {
            return Task.FromResult<ProductResponse?>(null);
        }

        return Task.FromResult<ProductResponse?>(ToResponse(product));
    }

    public Task<ProductResponse> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var id = Guid.NewGuid();
        var product = new Product(id, request.Name, request.Sku, request.Price, request.Description, request.IsActive);
        _items[id] = product;
        return Task.FromResult(ToResponse(product));
    }

    public Task<bool> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!_items.TryGetValue(id, out var current))
        {
            return Task.FromResult(false);
        }

        var updated = new Product(id, request.Name, request.Sku, request.Price, request.Description, request.IsActive);
        return Task.FromResult(_items.TryUpdate(id, updated, current));
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_items.TryRemove(id, out _));
    }

    private static ProductResponse ToResponse(Product product) =>
        new(product.Id, product.Name, product.Sku, product.Price, product.Description, product.IsActive);

    private sealed record Product(
        Guid Id,
        string Name,
        string Sku,
        decimal Price,
        string? Description,
        bool IsActive);
}
