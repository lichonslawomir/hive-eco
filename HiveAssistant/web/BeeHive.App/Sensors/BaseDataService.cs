using BeeHive.Domain.Aggregate;
using BeeHive.Domain.Data;
using Core.App;
using Microsoft.EntityFrameworkCore;

namespace BeeHive.App.Sensors;

public class BaseDataService
{
    protected readonly IBeeHiveDbContext _beeHiveDbContext;
    protected readonly IWorkContext _workContext;

    protected BaseDataService(IBeeHiveDbContext beeHiveDbContext, IWorkContext workContext)
    {
        _beeHiveDbContext = beeHiveDbContext;
        _workContext = workContext;
    }

    public async Task<(Domain.Holdings.Holding holding, bool newHolding)> GetHolding(string holdingKey, CancellationToken cancelationToken)
    {
        bool newHolding = false;
        var holding = await _beeHiveDbContext.Holdings.FirstOrDefaultAsync(x => x.UniqueKey == holdingKey, cancelationToken);
        if (holding is null)
        {
            newHolding = true;
            holding = Domain.Holdings.Holding.Create(holdingKey, holdingKey);
            await _beeHiveDbContext.Holdings.AddAsync(holding, cancelationToken);
        }
        return (holding, newHolding);
    }

    public async Task<(Domain.BeeGardens.BeeGarden beeGarden, bool newBeeGarden)> GetBeeGarden(string beeGardenKey, Domain.Holdings.Holding holding, bool newHolding,
        CancellationToken cancelationToken)
    {
        bool newBeeGarden = false;
        var beeGarden = newHolding ? null : await _beeHiveDbContext.BeeGardens.FirstOrDefaultAsync(x => x.UniqueKey == beeGardenKey && x.HoldingId == holding.Id, cancelationToken);
        if (beeGarden is null)
        {
            newBeeGarden = true;
            beeGarden = Domain.BeeGardens.BeeGarden.Create(beeGardenKey, beeGardenKey, _workContext.TimeZone(), holding);
            await _beeHiveDbContext.BeeGardens.AddAsync(beeGarden, cancelationToken);
        }
        return (beeGarden, newBeeGarden);
    }

    public async Task<(Domain.BeeGardens.BeeGarden beeGarden, bool newBeeGarden)> GetBeeGarden(string holdingKey, string beeGardenKey, CancellationToken cancelationToken)
    {
        var (holding, newHolding) = await GetHolding(holdingKey, cancelationToken);
        return await GetBeeGarden(beeGardenKey, holding, newHolding, cancelationToken);
    }

    public async Task<(Domain.Hives.Hive hive, bool newHive)> GetHive(string hievKey, Domain.BeeGardens.BeeGarden beeGarden, bool newBeeGarden, CancellationToken cancelationToken)
    {
        bool newHive = false;
        var hive = newBeeGarden ? null : await _beeHiveDbContext.Hives.FirstOrDefaultAsync(x => x.UniqueKey == hievKey && x.BeeGardenId == beeGarden.Id, cancelationToken);
        if (hive is null)
        {
            newHive = true;
            hive = Domain.Hives.Hive.Create(hievKey, hievKey, beeGarden);
            await _beeHiveDbContext.Hives.AddAsync(hive, cancelationToken);
        }

        return (hive, newHive);
    }

    public async Task<(TimeSeries timeSeries, bool newRecord)> GetTimeSeries(TimeSeriesKind kind, Domain.Hives.Hive hive, bool newHive, CancellationToken cancelationToken)
    {
        bool newRecord = false;
        var timeSeries = newHive ? null : await _beeHiveDbContext.TimeSeries.FirstOrDefaultAsync(x => x.Kind == kind && x.HiveId == hive.Id, cancelationToken);
        if (timeSeries is null)
        {
            newRecord = true;
            timeSeries = new TimeSeries(hive, kind);
            await _beeHiveDbContext.TimeSeries.AddAsync(timeSeries, cancelationToken);
        }
        return (timeSeries, newRecord);
    }

    public async Task<(TimeAggregateSeries aggregateSeries, bool newRecord)> GetAggregate(AggregationPeriod period, TimeSeries timeSeries, bool newTimeSeries, CancellationToken cancelationToken)
    {
        bool newRecord = false;
        var aggregateSeries = newTimeSeries ? null : await _beeHiveDbContext.TimeAggregateSeries.FirstOrDefaultAsync(x => x.Period == period && x.TimeSeriesId == timeSeries.Id, cancelationToken);
        if (aggregateSeries is null)
        {
            newRecord = true;
            aggregateSeries = new TimeAggregateSeries(timeSeries, period);
            await _beeHiveDbContext.TimeAggregateSeries.AddAsync(aggregateSeries, cancelationToken);
        }
        return (aggregateSeries, newRecord);
    }

    public async Task<TimeAggregateSeriesData?> GetAggregateData(DateTime ts, TimeAggregateSeries aggregateSeries, bool newAggregateSeries, CancellationToken cancelationToken)
    {
        return newAggregateSeries ? null :
            await _beeHiveDbContext.TimeAggregateSeriesData.FirstOrDefaultAsync(x => x.Timestamp == ts && x.TimeAggregateSeriesId == aggregateSeries.Id, cancelationToken);
    }

    public async Task<TimeSeriesData?> GetSeriesData(DateTime ts, TimeSeries timeSeries, bool newTimeSeries, CancellationToken cancelationToken)
    {
        return newTimeSeries ? null :
            await _beeHiveDbContext.TimeSeriesData.FirstOrDefaultAsync(x => x.Timestamp == ts && x.TimeSeriesId == timeSeries.Id, cancelationToken);
    }
}