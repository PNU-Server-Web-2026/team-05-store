using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OnlineStore.Api.Features.Warehouses;

/// <summary>
/// Конфігурація таблиці Warehouses: довжини рядків і обов'язковість полів.
/// </summary>
public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.ToTable("Warehouses", table => table.HasCheckConstraint(
            "CK_Warehouses_Priority",
            "\"Priority\" >= 1 AND \"Priority\" <= 100"));

        builder.HasKey(warehouse => warehouse.Id);

        builder.Property(warehouse => warehouse.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(warehouse => warehouse.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(warehouse => warehouse.Address)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(warehouse => warehouse.Priority)
            .IsRequired();
    }
}