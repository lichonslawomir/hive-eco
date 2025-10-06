using BeeHive.Domain.Data;
using Core.App;

namespace BeeHive.App.Sensors;

public interface ISensorService
{
    Task SaveData(string holdingKey, string beeGardenKey,
        IEnumerable<(string? hiveId, TimeSeriesKind seriesKind, float data, DateTime timestamp)> values,
        CancellationToken cancellationToken);
}

internal class SensorService : BaseDataService, ISensorService
{
    public SensorService(IBeeHiveDbContext beeHiveDbContext, IWorkContext workContext) : base(beeHiveDbContext, workContext)
    {
    }

    public async Task SaveData(string holdingKey, string beeGardenKey,
        IEnumerable<(string? hiveId, TimeSeriesKind seriesKind, float data, DateTime timestamp)> values,
        CancellationToken cancellationToken)
    {
        var (beeGarden, newBeeGarden) = await GetBeeGarden(holdingKey, beeGardenKey, cancellationToken);

        foreach (var kv in values.GroupBy(x => x.hiveId ?? string.Empty))
        {
            var hievKey = kv.Key;

            var (hive, newHive) = await GetHive(hievKey, beeGarden, newBeeGarden, cancellationToken);

            foreach (var d in kv.GroupBy(x => x.seriesKind))
            {
                var (timeSeries, _) = await GetTimeSeries(d.Key, hive, newHive, cancellationToken);
                timeSeries.AddData(d.Select(x => (x.timestamp, x.data)));
            }
        }

        await _beeHiveDbContext.SaveChangesAsync(cancellationToken);
    }
}