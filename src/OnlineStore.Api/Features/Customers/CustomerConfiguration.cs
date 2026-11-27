using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OnlineStore.Api.Features.Customers;

/// <summary>
/// Конфігурація таблиці Customers: довжини рядків і обов'язковість полів.
/// </summary>
public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(customer => customer.Id);

        builder.Property(customer => customer.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(customer => customer.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(customer => customer.Email)
            .IsRequired()
            .HasMaxLength(320);

        builder.Property(customer => customer.Phone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(customer => customer.ShippingAddress)
            .IsRequired()
            .HasMaxLength(300);
    }
}