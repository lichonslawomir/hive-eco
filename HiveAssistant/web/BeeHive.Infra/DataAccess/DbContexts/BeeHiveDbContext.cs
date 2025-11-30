using System.Reflection;
using BeeHive.App;
using BeeHive.Domain.Aggregate;
using BeeHive.Domain.BeeGardens;
using BeeHive.Domain.Data;
using BeeHive.Domain.Hives;
using BeeHive.Domain.Hives.Audio;
using BeeHive.Domain.Holdings;
using Core.App;
using Core.Infra.DataAccess.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BeeHive.Infra.DataAccess.DbContexts;

public sealed class BeeHiveDbContext : BaseDbContextManuallyVersioned<BeeHiveDbContext, string>, IBeeHiveDbContext
{
    private readonly IBeeHiveDbContextConfigurationProvider _configurationProvider;

    public BeeHiveDbContext(DbContextOptions<BeeHiveDbContext> options, IWorkContext workContext, IBeeHiveDbContextConfigurationProvider configurationProvider)
        : base(options, workContext)
    {
        _configurationProvider = configurationProvider;
    }

    public DbSet<Holding> Holdings { get; set; }

    public DbSet<BeeGarden> BeeGardens { get; set; }

    public DbSet<BeeGardenImportState> BeeGardenImportStates { get; set; }

    public DbSet<Hive> Hives { get; set; }

    public DbSet<HiveMedia> HiveMedia { get; set; }

    public DbSet<AudioFile> AudioFiles { get; set; }

    public DbSet<TimeSeries> TimeSeries { get; set; }

    public DbSet<TimeSeriesData> TimeSeriesData { get; set; }

    public DbSet<TimeAggregateSeries> TimeAggregateSeries { get; set; }

    public DbSet<TimeAggregateSeriesData> TimeAggregateSeriesData { get; set; }

    public DbSet<AudioAggregateStatsData> AudioAggregateStatsData { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        _configurationProvider.ApplyConfiguration(builder);
    }
}