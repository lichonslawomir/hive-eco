using BeeHive.Domain.Aggregate;
using BeeHive.Domain.BeeGardens;
using BeeHive.Domain.Data;
using BeeHive.Domain.Hives;
using BeeHive.Domain.Hives.Audio;
using BeeHive.Domain.Holdings;
using Microsoft.EntityFrameworkCore;

namespace BeeHive.App;

public interface IBeeHiveDbContext
{
    public DbSet<Holding> Holdings { get; }
    public DbSet<BeeGarden> BeeGardens { get; }
    public DbSet<BeeGardenImportState> BeeGardenImportStates { get; }
    public DbSet<Hive> Hives { get; }
    public DbSet<HiveMedia> HiveMedia { get; }
    public DbSet<TimeSeries> TimeSeries { get; }
    public DbSet<AudioFile> AudioFiles { get; }
    public DbSet<TimeSeriesData> TimeSeriesData { get; }
    public DbSet<TimeAggregateSeries> TimeAggregateSeries { get; }
    public DbSet<TimeAggregateSeriesData> TimeAggregateSeriesData { get; }
    public DbSet<AudioAggregateStatsData> AudioAggregateStatsData { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
}