using System.Collections.Concurrent;

namespace OnlineStore.Api.Features.Suppliers;

public sealed class InMemorySupplierService : ISupplierService
{
    private readonly ConcurrentDictionary<Guid, StoredSupplier> _items = new();

    public Task<IReadOnlyList<SupplierResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<SupplierResponse> result = _items.Values
            .OrderBy(supplier => supplier.Id)
            .Select(ToResponse)
            .ToArray();
        return Task.FromResult(result);
    }

    public Task<SupplierResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!_items.TryGetValue(id, out var supplier))
        {
            return Task.FromResult<SupplierResponse?>(null);
        }

        return Task.FromResult<SupplierResponse?>(ToResponse(supplier));
    }

    public Task<SupplierResponse> CreateAsync(CreateSupplierRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var id = Guid.NewGuid();
        var supplier = new StoredSupplier(id, request.Name, request.EdrpouCode, request.Email, request.Phone);
        _items[id] = supplier;
        return Task.FromResult(ToResponse(supplier));
    }

    public Task<bool> UpdateAsync(Guid id, UpdateSupplierRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!_items.TryGetValue(id, out var current))
        {
            return Task.FromResult(false);
        }

        var updated = new StoredSupplier(id, request.Name, request.EdrpouCode, request.Email, request.Phone);
        return Task.FromResult(_items.TryUpdate(id, updated, current));
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_items.TryRemove(id, out _));
    }

    private static SupplierResponse ToResponse(StoredSupplier supplier) =>
        new(supplier.Id, supplier.Name, supplier.EdrpouCode, supplier.Email, supplier.Phone);

    private sealed record StoredSupplier(Guid Id, string Name, string EdrpouCode, string? Email, string? Phone);
}
