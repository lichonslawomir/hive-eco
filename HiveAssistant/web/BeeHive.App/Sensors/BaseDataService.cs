using BeeHive.Domain.Data;
using BeeHive.Domain.Hives;
using Core.App;
using Microsoft.EntityFrameworkCore;

namespace BeeHive.App.Sensors;

internal class BaseDataService
{
    protected readonly IBeeHiveDbContext _beeHiveDbContext;
    protected readonly IWorkContext _workContext;

    public BaseDataService(IBeeHiveDbContext beeHiveDbContext, IWorkContext workContext)
    {
        _beeHiveDbContext = beeHiveDbContext;
        _workContext = workContext;
    }

    public async Task<(Domain.BeeGardens.BeeGarden beeGarden, bool newBeeGarden)> GetBeeGarden(string holdingKey, string beeGardenKey)
    {
        bool newHolding = false;
        var holding = await _beeHiveDbContext.Holdings.FirstOrDefaultAsync(x => x.UniqueKey == holdingKey);
        if (holding is null)
        {
            newHolding = true;
            holding = Domain.Holdings.Holding.Create(holdingKey, holdingKey);
            await _beeHiveDbContext.Holdings.AddAsync(holding);
        }

        bool newBeeGarden = false;
        var beeGarden = newHolding ? null : await _beeHiveDbContext.BeeGardens.FirstOrDefaultAsync(x => x.UniqueKey == beeGardenKey && x.HoldingId == holding.Id);
        if (beeGarden is null)
        {
            newBeeGarden = true;
            beeGarden = Domain.BeeGardens.BeeGarden.Create(beeGardenKey, beeGardenKey, _workContext.TimeZone(), holding);
            await _beeHiveDbContext.BeeGardens.AddAsync(beeGarden);
        }
        return (beeGarden, newBeeGarden);
    }

    public async Task<(Domain.Hives.Hive hive, bool newHive)> GetHive(string hievKey, Domain.BeeGardens.BeeGarden beeGarden, bool newBeeGarden)
    {
        bool newHive = false;
        var hive = newBeeGarden ? null : await _beeHiveDbContext.Hives.FirstOrDefaultAsync(x => x.UniqueKey == hievKey && x.BeeGardenId == beeGarden.Id);
        if (hive is null)
        {
            newHive = true;
            hive = Domain.Hives.Hive.Create(hievKey, hievKey, beeGarden);
            await _beeHiveDbContext.Hives.AddAsync(hive);
        }

        return (hive, newHive);
    }

    public async Task<TimeSeries> GetTimeSeries(TimeSeriesKind kind, Hive hive, bool newHive)
    {
        var timeSeries = newHive ? null : await _beeHiveDbContext.TimeSeries.FirstOrDefaultAsync(x => x.Kind == kind && x.HiveId == hive.Id);
        if (timeSeries is null)
        {
            timeSeries = new TimeSeries(hive, kind);
            await _beeHiveDbContext.TimeSeries.AddAsync(timeSeries);
        }
        return timeSeries;
    }
}