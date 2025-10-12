using BeeHive.Domain.Aggregate;
using BeeHive.Domain.Data;

namespace BeeHive.Contract.Export;

public struct TimeAggregateSeriesExportModel
{
    public struct Data
    {
        public required DateTimeOffset Timestamp { get; init; }

        public required int Count { get; init; }
        public required float? MaxValue { get; init; }
        public required float? MinValue { get; init; }
        public required float? AvgValue { get; init; }
        public required float? MedValue { get; init; }

        public required float? LastValue { get; init; }
        public required DateTimeOffset? LastValueTimestamp { get; init; }

        public required DateTimeOffset CreatedOrUpdatedDate { get; init; }
    }

    public required string HoldingUniqueKey { get; init; }
    public required string BeeGardenUniqueKey { get; init; }
    public required string HiveUniqueKey { get; init; }

    public required TimeSeriesKind Kind { get; init; }
    public required AggregationPeriod Period { get; init; }

    public required IReadOnlyCollection<Data> Datas { get; init; }
}