using BeeHive.App.Aggregate.Repositories;
using BeeHive.App.Aggregate.Repositories.Specifications;
using BeeHive.App.Data.Repositories;
using BeeHive.App.Data.Repositories.Specifications;
using BeeHive.App.Hives.Repositories;
using BeeHive.App.Hives.Repositories.Specifications;
using BeeHive.Contract.Aggregate.Models;
using BeeHive.Contract.Data.Models;
using BeeHive.Contract.Hives;
using BeeHive.Domain.Aggregate;
using BeeHive.Domain.Data;
using Hive.Gateway.Service.Models;
using Microsoft.Extensions.Options;

namespace Hive.Gateway.Service.Services;

public interface IHiveService
{
    Task<IList<HiveDto>> ListHives(CancellationToken cancellationToken = default);

    Task<IList<TimeSeriesDataModel>> GetHiveData(int hiveId,
            TimeSeriesKind kind,
            DateTime? start,
            DateTime? end,
            CancellationToken cancellationToken = default);

    Task<IList<TimeSeriesHivesDataModel>> GetHivesData(TimeSeriesKind kind,
            int[] hiveId,
            DateTime? start,
            DateTime? end,
            CancellationToken cancellationToken = default);

    Task<IList<TimeAggregateSeriesDataModel>> GetHiveAggregateData(int hiveId,
            TimeSeriesKind kind,
            AggregationPeriod period,
            DateTime? start,
            DateTime? end,
            CancellationToken cancellationToken = default);

    Task<IList<TimeAggregateSeriesHivesDataModel>> GetHivesAggregateData(TimeSeriesKind kind,
            AggregationPeriod period,
            int[] hiveId,
            DateTime? start,
            DateTime? end,
            CancellationToken cancellationToken = default);
}

public class HiveService(IOptions<BeeGardenConfig> beeGardenConfig,
    IHiveRepository hiveRepository,
    ITimeSeriesDataRepository timeSeriesDataRepository,
    ITimeAggregateSeriesDataRepository timeAggregateSeriesDataRepository) : IHiveService
{
    public async Task<IList<HiveDto>> ListHives(CancellationToken cancellationToken = default)
    {
        var spec = new HiveSpecification()
        {
            BeeGarden = (beeGardenConfig.Value.BeeGardenKey, beeGardenConfig.Value.HoldingKey)
        };

        return await hiveRepository.GetAsync<HiveDto>(spec, cancellationToken);
    }

    public async Task<IList<TimeSeriesDataModel>> GetHiveData(int hiveId,
            TimeSeriesKind kind,
            DateTime? start,
            DateTime? end,
            CancellationToken cancellationToken = default)
    {
        var spec = new TimeSeriesDataSpecification()
        {
            From = start,
            To = end,
            HiveId = hiveId,
            Kind = kind
        };
        return await timeSeriesDataRepository.GetAsync(spec, cancellationToken);
    }

    public async Task<IList<TimeSeriesHivesDataModel>> GetHivesData(TimeSeriesKind kind,
            int[] hiveId,
            DateTime? start,
            DateTime? end,
            CancellationToken cancellationToken = default)
    {
        var spec = new TimeSeriesHivesDataSpecification()
        {
            From = start,
            To = end,
            HiveIds = hiveId,
            Kind = kind
        };
        var linerData = await timeSeriesDataRepository.GetAsync(spec, cancellationToken);
        var list = new List<TimeSeriesHivesDataModel>(linerData.Count);

        int[] mapIdArray = new int[hiveId.Length];
        var sortId = hiveId.Distinct().Order().ToArray();
        for (int i = 0; i < sortId.Length; ++i)
        {
            mapIdArray[i] = Array.IndexOf(hiveId, sortId[i]);
        }

        DateTime? startDateTime = null;
        int valIdx = 0;
        foreach (var item in linerData)
        {
            if (!startDateTime.HasValue || startDateTime.Value != item.Timestamp)
            {
                startDateTime = item.Timestamp;
                valIdx = 0;
                list.Add(new TimeSeriesHivesDataModel()
                {
                    Timestamp = item.Timestamp,
                    Values = hiveId.Select(x => float.NaN).ToArray()
                });
            }

            while (hiveId[mapIdArray[valIdx]] != item.HiveId)
                ++valIdx;
            list[^1].Values[mapIdArray[valIdx]] = item.Value;
            ++valIdx;
        }

        return list;
    }

    public async Task<IList<TimeAggregateSeriesDataModel>> GetHiveAggregateData(int hiveId, TimeSeriesKind kind, AggregationPeriod period, DateTime? start, DateTime? end, CancellationToken cancellationToken = default)
    {
        var spec = new TimeAggregateSeriesDataSpecification()
        {
            From = start,
            To = end,
            HiveId = hiveId,
            Kind = kind
        };
        return await timeAggregateSeriesDataRepository.GetAsync(spec, cancellationToken);
    }

    public async Task<IList<TimeAggregateSeriesHivesDataModel>> GetHivesAggregateData(TimeSeriesKind kind, AggregationPeriod period, int[] hiveId, DateTime? start, DateTime? end, CancellationToken cancellationToken = default)
    {
        var spec = new TimeAggregateSeriesHivesDataSpecification()
        {
            From = start,
            To = end,
            HiveIds = hiveId,
            Kind = kind,
            Period = period
        };
        var linerData = await timeAggregateSeriesDataRepository.GetAsync(spec, cancellationToken);
        var list = new List<TimeAggregateSeriesHivesDataModel>(linerData.Count);

        int[] mapIdArray = new int[hiveId.Length];
        var sortId = hiveId.Distinct().Order().ToArray();
        for (int i = 0; i < sortId.Length; ++i)
        {
            mapIdArray[i] = Array.IndexOf(hiveId, sortId[i]);
        }

        DateTime? startDateTime = null;
        int valIdx = 0;
        foreach (var item in linerData)
        {
            if (!startDateTime.HasValue || startDateTime.Value != item.Timestamp)
            {
                startDateTime = item.Timestamp;
                valIdx = 0;

                list.Add(new TimeAggregateSeriesHivesDataModel()
                {
                    Timestamp = item.Timestamp,
                    Count = hiveId.Select(x => default(int)).ToArray(),
                    MaxValue = hiveId.Select(x => (float?)null).ToArray(),
                    MinValue = hiveId.Select(x => (float?)null).ToArray(),
                    AvgValue = hiveId.Select(x => (float?)null).ToArray(),
                    MedValue = hiveId.Select(x => (float?)null).ToArray()
                });
            }

            while (hiveId[mapIdArray[valIdx]] != item.HiveId)
                ++valIdx;
            list[^1].Count[mapIdArray[valIdx]] = item.Count;
            list[^1].MaxValue[mapIdArray[valIdx]] = item.MaxValue;
            list[^1].MinValue[mapIdArray[valIdx]] = item.MinValue;
            list[^1].AvgValue[mapIdArray[valIdx]] = item.AvgValue;
            list[^1].MedValue[mapIdArray[valIdx]] = item.MedValue;
            ++valIdx;
        }

        return list;
    }
}