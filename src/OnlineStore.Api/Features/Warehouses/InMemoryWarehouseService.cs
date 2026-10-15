using System.Collections.Concurrent;

namespace OnlineStore.Api.Features.Warehouses;

/// <summary>
/// Потокобезпечна реалізація сервісу складів у пам'яті (in-memory) з використанням ConcurrentDictionary.
/// </summary>
public class InMemoryWarehouseService : IWarehouseService
{
    private readonly ConcurrentDictionary<int, WarehouseResponse> _warehouses = new();
    private int _currentId;

    /// <inheritdoc />
    public Task<IReadOnlyList<WarehouseResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<WarehouseResponse> result = _warehouses.Values
            .OrderBy(w => w.Id)
            .ToList();

        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public Task<WarehouseResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        _warehouses.TryGetValue(id, out var warehouse);
        return Task.FromResult(warehouse);
    }

    /// <inheritdoc />
    public Task<WarehouseResponse> CreateAsync(CreateWarehouseRequest request, CancellationToken cancellationToken = default)
    {
        int id = Interlocked.Increment(ref _currentId);
        var warehouse = new WarehouseResponse(id, request.Name, request.City, request.Address, request.Priority);

        _warehouses[id] = warehouse;

        return Task.FromResult(warehouse);
    }

    /// <inheritdoc />
    public Task<bool> UpdateAsync(int id, UpdateWarehouseRequest request, CancellationToken cancellationToken = default)
    {
        if (!_warehouses.ContainsKey(id))
        {
            return Task.FromResult(false);
        }

        var updated = new WarehouseResponse(id, request.Name, request.City, request.Address, request.Priority);
        _warehouses[id] = updated;

        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        bool removed = _warehouses.TryRemove(id, out _);
        return Task.FromResult(removed);
    }
}
