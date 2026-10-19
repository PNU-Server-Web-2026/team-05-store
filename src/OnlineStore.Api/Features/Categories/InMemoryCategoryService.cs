using System.Collections.Concurrent;

namespace OnlineStore.Api.Features.Categories;

public sealed class InMemoryCategoryService : ICategoryService
{
    private readonly ConcurrentDictionary<int, Category> _items = new();
    private int _nextId;

    public async Task<IReadOnlyList<CategoryResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<CategoryResponse> result = _items.Values
            .OrderBy(category => category.Id)
            .Select(ToResponse)
            .ToArray();
        return await Task.FromResult(result);
    }

    public async Task<CategoryResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!_items.TryGetValue(id, out var category))
        {
            return null;
        }

        return await Task.FromResult(ToResponse(category));
    }

    public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var id = Interlocked.Increment(ref _nextId);
        var category = new Category(id, request.Name, request.Slug, request.Description);
        _items[id] = category;
        return await Task.FromResult(ToResponse(category));
    }

    public async Task<bool> UpdateAsync(int id, UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!_items.TryGetValue(id, out _))
        {
            return false;
        }

        _items[id] = new Category(id, request.Name, request.Slug, request.Description);
        return await Task.FromResult(true);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await Task.FromResult(_items.TryRemove(id, out _));
    }

    private static CategoryResponse ToResponse(Category category) =>
        new(category.Id, category.Name, category.Slug, category.Description);

    private sealed record Category(int Id, string Name, string Slug, string Description);
}
