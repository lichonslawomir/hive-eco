using BeeHive.Domain.Data;
using Core.App;

namespace BeeHive.App.Sensors;

public struct SensorData
{
    public required string? HiveId { get; init; }
    public required string ComPort { get; init; }
    public required TimeSeriesKind SeriesKind { get; init; }
    public required float Data { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
}

public interface ISensorService
{
    Task SaveData(string holdingKey, string beeGardenKey,
        IEnumerable<SensorData> values,
        CancellationToken cancellationToken);
}

internal class SensorService : BaseDataService, ISensorService
{
    public SensorService(IBeeHiveDbContext beeHiveDbContext, IWorkContext workContext) : base(beeHiveDbContext, workContext)
    {
    }

    public async Task SaveData(string holdingKey, string beeGardenKey,
        IEnumerable<SensorData> values,
        CancellationToken cancellationToken)
    {
        var (beeGarden, newBeeGarden) = await GetBeeGarden(holdingKey, beeGardenKey, cancellationToken);

        foreach (var kv in values.GroupBy(x => x.HiveId ?? string.Empty))
        {
            var hievKey = kv.Key;

            var (hive, newHive) = await GetHive(hievKey, beeGarden, newBeeGarden, cancellationToken);

            foreach (var d in kv.GroupBy(x => x.SeriesKind))
            {
                var (timeSeries, _) = await GetTimeSeries(d.Key, hive, newHive, cancellationToken);
                timeSeries.AddData(d.Select(x => (x.Timestamp.UtcDateTime, x.Data)));
                hive.UpdateComPort(d.Last().ComPort);
            }
        }
        await _beeHiveDbContext.SaveChangesAsync(cancellationToken);
    }
}