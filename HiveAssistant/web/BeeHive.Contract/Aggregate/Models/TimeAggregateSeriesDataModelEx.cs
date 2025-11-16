using BeeHive.Domain.Aggregate;
using BeeHive.Domain.Data;

namespace BeeHive.Contract.Aggregate.Models;

public struct TimeAggregateSeriesDataModelEx
{
    public required int HiveId { get; init; }
    public required TimeSeriesKind Kind { get; init; }
    public required AggregationPeriod Period { get; init; }

    public DateTimeOffset Timestamp { get; init; }

    public required int Count { get; init; }
    public float? MaxValue { get; init; }
    public float? MinValue { get; init; }
    public float? AvgValue { get; init; }
    public float? MedValue { get; init; }

    public DateTimeOffset CreatedOrUpdatedDate { get; init; }
}