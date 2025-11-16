namespace BeeHive.Contract.Aggregate.Models;

public struct TimeAggregateSeriesHivesDataModel
{
    public required DateTimeOffset Timestamp { get; init; }
    public required int[] Count { get; init; }
    public float?[] MaxValue { get; init; }
    public float?[] MinValue { get; init; }
    public float?[] AvgValue { get; init; }
    public float?[] MedValue { get; init; }
}