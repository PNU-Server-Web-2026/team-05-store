using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OnlineStore.Api.Features.Products;

/// <summary>
/// Конфігурація таблиці Products: довжини рядків, обов'язковість і точність ціни.
/// </summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(product => product.Id);

        builder.Property(product => product.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(product => product.Sku)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(product => product.Price)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(product => product.Description)
            .HasMaxLength(4000);

        builder.Property(product => product.IsActive)
            .IsRequired();
    }
}
