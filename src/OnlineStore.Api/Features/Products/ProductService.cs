using Microsoft.EntityFrameworkCore;
using OnlineStore.Api.Data;

namespace OnlineStore.Api.Features.Products;

public sealed class ProductService(AppDbContext db) : IProductService
{
    public async Task<IReadOnlyList<ProductResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await db.Products
            .AsNoTracking()
            .OrderBy(product => product.Name)
            .Select(product => new ProductResponse(
                product.Id,
                product.Name,
                product.Sku,
                product.Price,
                product.Description,
                product.IsActive))
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await db.Products
            .AsNoTracking()
            .Where(product => product.Id == id)
            .Select(product => new ProductResponse(
                product.Id,
                product.Name,
                product.Sku,
                product.Price,
                product.Description,
                product.IsActive))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ProductResponse> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken)
    {
        var entity = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Sku = request.Sku,
            Price = request.Price,
            Description = request.Description,
            IsActive = request.IsActive
        };

        db.Products.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return new ProductResponse(
            entity.Id,
            entity.Name,
            entity.Sku,
            entity.Price,
            entity.Description,
            entity.IsActive);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var entity = await db.Products
            .FirstOrDefaultAsync(product => product.Id == id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        entity.Name = request.Name;
        entity.Sku = request.Sku;
        entity.Price = request.Price;
        entity.Description = request.Description;
        entity.IsActive = request.IsActive;

        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await db.Products
            .FirstOrDefaultAsync(product => product.Id == id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        db.Products.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }
}