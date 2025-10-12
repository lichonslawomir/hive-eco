namespace BeeHive.Contract.Aggregate.Models;

public class TimeAggregateSeriesDataModel
{
    public required DateTime Timestamp { get; init; }
    public required int Count { get; init; }
    public required float? MaxValue { get; init; }
    public required float? MinValue { get; init; }
    public required float? AvgValue { get; init; }
    public required float? MedValue { get; init; }
}