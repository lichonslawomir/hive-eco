using System.Collections.Concurrent;
using BeeHive.Contract.Export;
using BeeHive.Domain.BeeGardens;
using Hive.Gateway.Service.Models;
using Microsoft.Extensions.Options;

namespace Hive.Gateway.Service.Export;

public interface IExportService
{
    Task<DateTimeOffset?> GetLastExportDate(ExportEntity exportEntity, CancellationToken stoppingToken);

    Task Export(IReadOnlyCollection<HiveExportModel> hiveExportModels, DateTimeOffset lastExportData, CancellationToken stoppingToken);

    Task Export(IReadOnlyCollection<HiveMediaExportModel> hiveExportModels, DateTimeOffset lastExportData, CancellationToken stoppingToken);

    Task Export(IReadOnlyCollection<TimeAggregateSeriesExportModel> hiveExportModels, DateTimeOffset lastExportData, CancellationToken stoppingToken);
}

public class ExportService(HttpClient client, IOptions<BeeGardenConfig> beeGardenConfig, IConfiguration configuration) : IExportService
{
    private ConcurrentDictionary<ExportEntity, DateTimeOffset> _cache = new ConcurrentDictionary<ExportEntity, DateTimeOffset>();

    public async Task<DateTimeOffset?> GetLastExportDate(ExportEntity exportEntity, CancellationToken stoppingToken)
    {
        var exportSecret = configuration["ExportSecret"];
#if !DEBUG
        if (exportSecret == "...")
            return DateTimeOffset.MaxValue;
#endif

        if (_cache.TryGetValue(exportEntity, out DateTimeOffset lastExportData))
            return lastExportData;
        var returnValue = await client.GetFromJsonAsync<ExportState>($"api/export/state/{exportEntity}?holding={beeGardenConfig.Value.HoldingKey}&beeGarden={beeGardenConfig.Value.BeeGardenKey}", stoppingToken);
        if (returnValue.State.HasValue)
            _cache[exportEntity] = returnValue.State.Value;
        return returnValue.State;
    }

    public async Task Export(IReadOnlyCollection<HiveExportModel> hiveExportModels, DateTimeOffset lastExportData, CancellationToken stoppingToken)
    {
        var response = await client.PostAsJsonAsync("api/export/hives", hiveExportModels, stoppingToken);
        response.EnsureSuccessStatusCode();
        _cache[ExportEntity.Hive] = lastExportData;
    }

    public async Task Export(IReadOnlyCollection<HiveMediaExportModel> hiveExportModels, DateTimeOffset lastExportData, CancellationToken stoppingToken)
    {
        var response = await client.PostAsJsonAsync("api/export/hive-medias", hiveExportModels, stoppingToken);
        response.EnsureSuccessStatusCode();
        _cache[ExportEntity.HiveMedia] = lastExportData;
    }

    public async Task Export(IReadOnlyCollection<TimeAggregateSeriesExportModel> exportModels, DateTimeOffset lastExportData, CancellationToken stoppingToken)
    {
        var response = await client.PostAsJsonAsync("api/export/hive-data", exportModels, stoppingToken);
        response.EnsureSuccessStatusCode();
        _cache[ExportEntity.TimeTimeAggregateSeriesData] = lastExportData;
    }
}