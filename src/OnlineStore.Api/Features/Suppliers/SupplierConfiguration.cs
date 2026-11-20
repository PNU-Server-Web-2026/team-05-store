using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OnlineStore.Api.Features.Suppliers;

/// <summary>
/// Конфігурація таблиці Suppliers: довжини рядків і обов'язковість полів.
/// </summary>
public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("Suppliers");

        builder.HasKey(supplier => supplier.Id);

        builder.Property(supplier => supplier.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(supplier => supplier.EdrpouCode)
            .IsRequired()
            .HasMaxLength(8);

        builder.Property(supplier => supplier.Email)
            .HasMaxLength(256);

        builder.Property(supplier => supplier.Phone)
            .HasMaxLength(16);
    }
}
