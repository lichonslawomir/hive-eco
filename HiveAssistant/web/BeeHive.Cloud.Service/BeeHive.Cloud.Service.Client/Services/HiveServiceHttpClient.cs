using System.Net.Http.Json;
using System.Text;
using BeeHive.Contract.Aggregate.Models;
using BeeHive.Contract.Data.Models;
using BeeHive.Contract.Hives.Models;
using BeeHive.Contract.Interfaces;
using BeeHive.Domain.Aggregate;
using BeeHive.Domain.Data;

namespace BeeHive.Cloud.Service.Client.Services;

public class HiveServiceHttpClient(HttpClient httpClient) : IHiveService
{
    private const string urlConst = "api/hives";

    public async Task<HiveDto> GetHive(int id, CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<HiveDto>($"{urlConst}/{id}", cancellationToken);
    }

    public async Task<IList<HiveDto>> ListHives(CancellationToken cancellationToken = default)
    {
        var result = await httpClient.GetFromJsonAsync<IList<HiveDto>>($"{urlConst}", cancellationToken);
        return result ?? new List<HiveDto>();
    }

    public async Task<IList<TimeAggregateSeriesDataModel>> GetHiveAggregateData(int hiveId, TimeSeriesKind kind, AggregationPeriod period, DateTimeOffset? start, DateTimeOffset? end, CancellationToken cancellationToken = default)
    {
        var url = new StringBuilder();
        if (start.HasValue)
            url.Append((url.Length == 0 ? "?" : "&") + $"start={start.Value.UtcDateTime.ToString("o")}");
        if (end.HasValue)
            url.Append((url.Length == 0 ? "?" : "&") + $"end={end.Value.UtcDateTime.ToString("o")}");
        var result = await httpClient.GetFromJsonAsync<IList<TimeAggregateSeriesDataModel>>(
            $"{urlConst}/{hiveId}/aggregate-data/{kind}/{period}{url.ToString()}",
            cancellationToken);
        return result ?? new List<TimeAggregateSeriesDataModel>();
    }

    public async Task<IList<TimeSeriesDataModel>> GetHiveData(int hiveId, TimeSeriesKind kind, DateTimeOffset? start, DateTimeOffset? end, CancellationToken cancellationToken = default)
    {
        var url = new StringBuilder();
        if (start.HasValue)
            url.Append((url.Length == 0 ? "?" : "&") + $"start={start.Value.UtcDateTime.ToString("o")}");
        if (end.HasValue)
            url.Append((url.Length == 0 ? "?" : "&") + $"end={end.Value.UtcDateTime.ToString("o")}");
        var result = await httpClient.GetFromJsonAsync<IList<TimeSeriesDataModel>>(
            $"{urlConst}/{hiveId}/data/{kind}{url.ToString()}",
            cancellationToken);
        return result ?? new List<TimeSeriesDataModel>();
    }

    public async Task<TimeSeriesDataModel?> GetHiveLastData(int hiveId, TimeSeriesKind kind, CancellationToken cancellationToken = default)
    {
        var result = await httpClient.GetFromJsonAsync<TimeSeriesDataModel>(
            $"{urlConst}/{hiveId}/last-data/{kind}",
            cancellationToken);
        return result;
    }

    public async Task<IList<TimeAggregateSeriesHivesDataModel>> GetHivesAggregateData(TimeSeriesKind kind,
        AggregationPeriod period,
        int[] hiveId,
        DateTimeOffset? start,
        DateTimeOffset? end,
        CancellationToken cancellationToken = default)
    {
        var url = new StringBuilder();
        if (start.HasValue)
            url.Append((url.Length == 0 ? "?" : "&") + $"start={start.Value.UtcDateTime.ToString("o")}");
        if (end.HasValue)
            url.Append((url.Length == 0 ? "?" : "&") + $"end={end.Value.UtcDateTime.ToString("o")}");
        foreach (var id in hiveId)
            url.Append((url.Length == 0 ? "?" : "&") + $"hiveId={id}");
        var result = await httpClient.GetFromJsonAsync<IList<TimeAggregateSeriesHivesDataModel>>(
            $"{urlConst}/aggregate-data/{kind}/{period}{url.ToString()}",
            cancellationToken);
        return result ?? new List<TimeAggregateSeriesHivesDataModel>();
    }

    public async Task<IList<TimeSeriesHivesDataModel>> GetHivesData(TimeSeriesKind kind, int[] hiveId, DateTimeOffset? start, DateTimeOffset? end, CancellationToken cancellationToken = default)
    {
        var url = new StringBuilder();
        if (start.HasValue)
            url.Append((url.Length == 0 ? "?" : "&") + $"start={start.Value.UtcDateTime.ToString("o")}");
        if (end.HasValue)
            url.Append((url.Length == 0 ? "?" : "&") + $"end={end.Value.UtcDateTime.ToString("o")}");
        foreach (var id in hiveId)
            url.Append((url.Length == 0 ? "?" : "&") + $"hiveId={id}");
        var result = await httpClient.GetFromJsonAsync<IList<TimeSeriesHivesDataModel>>(
            $"{urlConst}/data/{kind}{url.ToString()}",
            cancellationToken);
        return result ?? new List<TimeSeriesHivesDataModel>();
    }
}