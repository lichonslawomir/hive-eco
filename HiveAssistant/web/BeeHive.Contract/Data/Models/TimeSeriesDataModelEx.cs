namespace BeeHive.Contract.Data.Models;

public struct TimeSeriesDataModelEx
{
    public required int HiveId { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public required float Value { get; init; }
}