using BeeHive.Domain.Data;
using Core.Infra.DataAccess.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeeHive.Infra.DataAccess.Mappings;

internal sealed class TimeSeriesDataConfiguration : IEntityTypeConfiguration<TimeSeriesData>
{
    public void Configure(EntityTypeBuilder<TimeSeriesData> builder)
    {
        builder.HasKey(e => new { e.TimeSeriesId, e.Timestamp });

        builder.Property(e => e.Timestamp)
            .UtcDateTime()
            .IsRequired();

        builder.Property(e => e.Value)
            .IsRequired();
    }
}