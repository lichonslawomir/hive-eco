using BeeHive.Domain.BeeGardens;
using Core.Infra.DataAccess.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeeHive.Infra.Sqlite.Mappings;

internal sealed class BeeGardenConfiguration : EntityConfiguration<BeeGarden, int>, IEntityTypeConfiguration<BeeGarden>
{
    public void Configure(EntityTypeBuilder<BeeGarden> builder)
    {
        ConfigureEntity(builder);
        ConfigureAudited<BeeGarden, string>(builder);

        builder.Property(e => e.UniqueKey)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(e => e.Name)
            .HasMaxLength(255)
            .IsRequired();
        builder.Property(e => e.TimeZone)
            .HasMaxLength(255)
            .IsRequired();

        builder.HasOne(e => e.Holding)
            .WithMany()
            .HasForeignKey(e => e.HoldingId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(true);

        builder.HasIndex(e => new { e.HoldingId, e.UniqueKey })
            .IsUnique(true);
    }
}