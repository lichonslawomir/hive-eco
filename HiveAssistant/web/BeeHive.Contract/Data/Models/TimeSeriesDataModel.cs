namespace BeeHive.Contract.Data.Models;

public class TimeSeriesDataModel
{
    public required DateTime Timestamp { get; init; }
    public required float Value { get; init; }
}