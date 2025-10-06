using System.Collections.Concurrent;
using BeeHive.Contract.Export;
using BeeHive.Domain.BeeGardens;
using Hive.Gateway.Service.Models;
using Microsoft.Extensions.Options;

namespace Hive.Gateway.Service.Export;

public interface IExportService
{
    Task<DateTime?> GetLastExportDate(ExportEntity exportEntity, CancellationToken stoppingToken);

    Task Export(IReadOnlyCollection<HiveExportModel> hiveExportModels, DateTime lastExportData, CancellationToken stoppingToken);

    Task Export(IReadOnlyCollection<HiveMediaExportModel> hiveExportModels, DateTime lastExportData, CancellationToken stoppingToken);

    Task Export(IReadOnlyCollection<TimeAggregateSeriesExportModel> hiveExportModels, DateTime lastExportData, CancellationToken stoppingToken);
}

public class ExportService(HttpClient client, IOptions<BeeGardenConfig> beeGardenConfig) : IExportService
{
    private ConcurrentDictionary<ExportEntity, DateTime> _cache = new ConcurrentDictionary<ExportEntity, DateTime>();

    public async Task<DateTime?> GetLastExportDate(ExportEntity exportEntity, CancellationToken stoppingToken)
    {
        if (_cache.TryGetValue(exportEntity, out DateTime lastExportData))
            return lastExportData;
        var returnValue = await client.GetFromJsonAsync<ExportState>($"api/export/state/{exportEntity}?holding={beeGardenConfig.Value.HoldingKey}&beeGarden={beeGardenConfig.Value.BeeGardenKey}");
        if (returnValue.State.HasValue)
            _cache[exportEntity] = returnValue.State.Value;
        return returnValue.State;
    }

    public async Task Export(IReadOnlyCollection<HiveExportModel> hiveExportModels, DateTime lastExportData, CancellationToken stoppingToken)
    {
        var response = await client.PostAsJsonAsync("api/export/hives", hiveExportModels);
        response.EnsureSuccessStatusCode();
        _cache[ExportEntity.Hive] = lastExportData;
    }

    public async Task Export(IReadOnlyCollection<HiveMediaExportModel> hiveExportModels, DateTime lastExportData, CancellationToken stoppingToken)
    {
        var response = await client.PostAsJsonAsync("api/export/hive-medias", hiveExportModels);
        response.EnsureSuccessStatusCode();
        _cache[ExportEntity.HiveMedia] = lastExportData;
    }

    public async Task Export(IReadOnlyCollection<TimeAggregateSeriesExportModel> exportModels, DateTime lastExportData, CancellationToken stoppingToken)
    {
        var response = await client.PostAsJsonAsync("api/export/hive-data", exportModels);
        response.EnsureSuccessStatusCode();
        _cache[ExportEntity.TimeTimeAggregateSeriesData] = lastExportData;
    }
}