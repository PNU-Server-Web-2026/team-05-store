using System.Collections.Concurrent;

namespace OnlineStore.Api.Features.Categories;

public sealed class InMemoryCategoryService : ICategoryService
{
    private readonly ConcurrentDictionary<Guid, Category> _items = new();

    public Task<IReadOnlyList<CategoryResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<CategoryResponse> result = _items.Values
            .OrderBy(category => category.Id)
            .Select(ToResponse)
            .ToArray();
        return Task.FromResult(result);
    }

    public Task<CategoryResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!_items.TryGetValue(id, out var category))
        {
            return Task.FromResult<CategoryResponse?>(null);
        }

        return Task.FromResult<CategoryResponse?>(ToResponse(category));
    }

    public Task<CategoryResponse> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var id = Guid.NewGuid();
        var category = new Category(id, request.Name, request.Slug, request.Description);
        _items[id] = category;
        return Task.FromResult(ToResponse(category));
    }

    public Task<bool> UpdateAsync(Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!_items.TryGetValue(id, out var current))
        {
            return Task.FromResult(false);
        }

        var updated = new Category(id, request.Name, request.Slug, request.Description);
        return Task.FromResult(_items.TryUpdate(id, updated, current));
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_items.TryRemove(id, out _));
    }

    private static CategoryResponse ToResponse(Category category) =>
        new(category.Id, category.Name, category.Slug, category.Description);

    private sealed record Category(Guid Id, string Name, string Slug, string? Description);
}
