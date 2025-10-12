using BeeHive.Domain.Data;
using Core.Infra.DataAccess.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeeHive.Infra.DataAccess.Mappings;

internal sealed class TimeSeriesConfiguration : EntityConfiguration<TimeSeries, int>, IEntityTypeConfiguration<TimeSeries>
{
    public void Configure(EntityTypeBuilder<TimeSeries> builder)
    {
        ConfigureEntity(builder);

        builder.Property(e => e.Kind)
            .IsRequired();

        builder.HasMany(e => e.Data)
            .WithOne(f => f.TimeSeries)
            .HasForeignKey(e => e.TimeSeriesId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(true);

        builder.HasOne(e => e.Hive)
            .WithMany()
            .HasForeignKey(e => e.HiveId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(true);

        builder.HasIndex(e => new { e.HiveId, e.Kind })
            .IsUnique(true);
    }
}