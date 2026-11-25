using Microsoft.EntityFrameworkCore;
using OnlineStore.Api.Data;

namespace OnlineStore.Api.Features.Categories;

public sealed class CategoryService(AppDbContext db) : ICategoryService
{
    public async Task<IReadOnlyList<CategoryResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await db.Categories
            .AsNoTracking()
            .OrderBy(category => category.Id)
            .Select(category => new CategoryResponse(
                category.Id,
                category.Name,
                category.Slug,
                category.Description))
            .ToListAsync(cancellationToken);
    }

    public async Task<CategoryResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await db.Categories
            .AsNoTracking()
            .Where(category => category.Id == id)
            .Select(category => new CategoryResponse(
                category.Id,
                category.Name,
                category.Slug,
                category.Description))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var entity = new Category
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Slug = request.Slug,
            Description = request.Description
        };

        db.Categories.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return new CategoryResponse(entity.Id, entity.Name, entity.Slug, entity.Description);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var entity = await db.Categories
            .FirstOrDefaultAsync(category => category.Id == id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        entity.Name = request.Name;
        entity.Slug = request.Slug;
        entity.Description = request.Description;

        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await db.Categories
            .FirstOrDefaultAsync(category => category.Id == id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        db.Categories.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
