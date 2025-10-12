namespace BeeHive.Contract.Data.Models;

public class TimeSeriesDataModelEx
{
    public required int HiveId { get; init; }
    public required DateTime Timestamp { get; init; }
    public required float Value { get; init; }
}