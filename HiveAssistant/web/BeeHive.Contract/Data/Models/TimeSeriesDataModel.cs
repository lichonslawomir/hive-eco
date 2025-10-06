namespace BeeHive.Contract.Data.Models;

public struct TimeSeriesDataModel
{
    public required DateTime Timestamp { get; init; }
    public required float Value { get; init; }
}