using BeeHive.Domain.Aggregate;
using Core.Infra.DataAccess.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeeHive.Infra.DataAccess.Mappings;

internal sealed class TimeAggregateSeriesConfiguration : EntityConfiguration<TimeAggregateSeries, int>, IEntityTypeConfiguration<TimeAggregateSeries>
{
    public void Configure(EntityTypeBuilder<TimeAggregateSeries> builder)
    {
        ConfigureEntity(builder);

        builder.Property(e => e.Period)
            .IsRequired();
        builder.Property(e => e.LasteAggregateTime)
            .IsRequired(false);

        builder.HasOne(e => e.TimeSeries)
            .WithMany()
            .HasForeignKey(e => e.TimeSeriesId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(true);

        builder.HasIndex(e => new { e.TimeSeriesId, e.Period })
            .IsUnique(true);
    }
}