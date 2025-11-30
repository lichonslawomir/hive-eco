using BeeHive.Domain.Aggregate;
using Core.Infra.DataAccess.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeeHive.Infra.Sqlite.Mappings;

internal sealed class AudioAggregateStatsDataConfiguration : IEntityTypeConfiguration<AudioAggregateStatsData>
{
    public void Configure(EntityTypeBuilder<AudioAggregateStatsData> builder)
    {
        builder.HasKey(e => new { e.TimeAggregateSeriesId, e.Timestamp });

        builder.HasOne(e => e.TimeAggregateSeries)
            .WithMany(f => f.AudioStats)
            .HasForeignKey(e => e.TimeAggregateSeriesId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(true);

        builder.Property(e => e.DurationSec)
            .IsRequired();
        builder.Property(e => e.Frequency)
            .IsRequired();
        builder.Property(e => e.AmplitudePeak)
            .IsRequired();
        builder.Property(e => e.AmplitudeRms)
            .IsRequired();
        builder.Property(e => e.AmplitudeMav)
            .IsRequired();

        builder.Property(e => e.Timestamp)
            .UtcDateTime();
    }
}