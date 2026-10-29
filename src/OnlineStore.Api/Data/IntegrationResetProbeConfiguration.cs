using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OnlineStore.Api.Data;

internal sealed class IntegrationResetProbeConfiguration : IEntityTypeConfiguration<IntegrationResetProbe>
{
    public void Configure(EntityTypeBuilder<IntegrationResetProbe> builder)
    {
        builder.ToTable("integration_reset_probes");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Label).HasMaxLength(100).IsRequired();
    }
}
