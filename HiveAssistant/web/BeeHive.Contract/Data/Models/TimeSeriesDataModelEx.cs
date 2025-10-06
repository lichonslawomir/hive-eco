namespace BeeHive.Contract.Data.Models;

public struct TimeSeriesDataModelEx
{
    public required int HiveId { get; init; }
    public required DateTime Timestamp { get; init; }
    public required float Value { get; init; }
}