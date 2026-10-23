using System.Collections.Concurrent;

namespace OnlineStore.Api.Features.Warehouses;

/// <summary>
/// Потокобезпечна реалізація сервісу складів у пам'яті (in-memory) з використанням ConcurrentDictionary.
/// </summary>
public class InMemoryWarehouseService : IWarehouseService
{
    private readonly ConcurrentDictionary<Guid, Warehouse> _warehouses = new();

    /// <inheritdoc />
    public Task<IReadOnlyList<WarehouseResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<WarehouseResponse> result = _warehouses.Values
            .OrderBy(w => w.Id)
            .Select(ToResponse)
            .ToArray();

        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public Task<WarehouseResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!_warehouses.TryGetValue(id, out var warehouse))
        {
            return Task.FromResult<WarehouseResponse?>(null);
        }

        return Task.FromResult<WarehouseResponse?>(ToResponse(warehouse));
    }

    /// <inheritdoc />
    public Task<WarehouseResponse> CreateAsync(CreateWarehouseRequest request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var id = Guid.NewGuid();
        var warehouse = new Warehouse(id, request.Name, request.City, request.Address, request.Priority);

        _warehouses[id] = warehouse;

        return Task.FromResult(ToResponse(warehouse));
    }

    /// <inheritdoc />
    public Task<bool> UpdateAsync(Guid id, UpdateWarehouseRequest request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!_warehouses.TryGetValue(id, out var current))
        {
            return Task.FromResult(false);
        }

        var updated = new Warehouse(id, request.Name, request.City, request.Address, request.Priority);
        return Task.FromResult(_warehouses.TryUpdate(id, updated, current));
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_warehouses.TryRemove(id, out _));
    }

    private static WarehouseResponse ToResponse(Warehouse warehouse) =>
        new(warehouse.Id, warehouse.Name, warehouse.City, warehouse.Address, warehouse.Priority);

    private sealed record Warehouse(Guid Id, string Name, string City, string Address, int Priority);
}
