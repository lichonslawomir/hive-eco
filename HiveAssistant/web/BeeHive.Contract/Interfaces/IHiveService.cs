using BeeHive.Contract.Aggregate.Models;
using BeeHive.Contract.Data.Models;
using BeeHive.Contract.Hives.Models;
using BeeHive.Domain.Aggregate;
using BeeHive.Domain.Data;

namespace BeeHive.Contract.Interfaces;

public interface IHiveService
{
    Task<IList<HiveDto>> ListHives(CancellationToken cancellationToken = default);

    Task<HiveDto> GetHive(int id, CancellationToken cancellationToken = default);

    Task<TimeSeriesDataModel?> GetHiveLastData(int hiveId,
            TimeSeriesKind kind,
            CancellationToken cancellationToken = default);

    Task<IList<TimeSeriesDataModel>> GetHiveData(int hiveId,
            TimeSeriesKind kind,
            DateTimeOffset? start,
            DateTimeOffset? end,
            CancellationToken cancellationToken = default);

    Task<IList<TimeSeriesHivesDataModel>> GetHivesData(TimeSeriesKind kind,
            int[] hiveId,
            DateTimeOffset? start,
            DateTimeOffset? end,
            CancellationToken cancellationToken = default);

    Task<IList<TimeAggregateSeriesDataModel>> GetHiveAggregateData(int hiveId,
            TimeSeriesKind kind,
            AggregationPeriod period,
            DateTimeOffset? start,
            DateTimeOffset? end,
            CancellationToken cancellationToken = default);

    Task<IList<TimeAggregateSeriesHivesDataModel>> GetHivesAggregateData(TimeSeriesKind kind,
            AggregationPeriod period,
            int[] hiveId,
            DateTimeOffset? start,
            DateTimeOffset? end,
            CancellationToken cancellationToken = default);
}