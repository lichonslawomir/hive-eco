using BeeHive.Domain.Aggregate;
using BeeHive.Domain.Data;

namespace BeeHive.Contract.Aggregate.Models;

public class TimeAggregateSeriesDataModelEx
{
    public required int HiveId { get; init; }
    public required TimeSeriesKind Kind { get; init; }
    public required AggregationPeriod Period { get; init; }

    public DateTimeOffset Timestamp { get; init; }

    public required int Count { get; init; }
    public required float? MaxValue { get; init; }
    public required float? MinValue { get; init; }
    public required float? AvgValue { get; init; }
    public required float? MedValue { get; init; }

    public DateTimeOffset CreatedOrUpdatedDate { get; init; }
}