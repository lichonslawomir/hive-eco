namespace BeeHive.Contract.Data.Models;

public struct TimeSeriesDataModel
{
    public required DateTimeOffset Timestamp { get; init; }
    public required float Value { get; init; }
}