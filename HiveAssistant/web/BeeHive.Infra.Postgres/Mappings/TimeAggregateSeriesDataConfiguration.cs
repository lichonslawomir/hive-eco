using BeeHive.Domain.Aggregate;
using Core.Infra.DataAccess.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeeHive.Infra.Sqlite.Mappings;

internal sealed class TimeAggregateSeriesDataConfiguration : IEntityTypeConfiguration<TimeAggregateSeriesData>
{
    public void Configure(EntityTypeBuilder<TimeAggregateSeriesData> builder)
    {
        builder.ConfigureSynchronizable();

        builder.HasKey(e => new { e.TimeAggregateSeriesId, e.Timestamp });

        builder.HasOne(e => e.TimeAggregateSeries)
            .WithMany(f => f.Data)
            .HasForeignKey(e => e.TimeAggregateSeriesId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(true);

        builder.Property(e => e.Timestamp)
            .UtcDateTime()
            .IsRequired();

        builder.Property(e => e.Count)
            .IsRequired();
        builder.Property(e => e.MinValue)
            .IsRequired(false);
        builder.Property(e => e.MaxValue)
            .IsRequired(false);
        builder.Property(e => e.AvgValue)
            .IsRequired(false);
        builder.Property(e => e.MedValue)
            .IsRequired(false);
    }
}