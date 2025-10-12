namespace BeeHive.Contract.Data.Models;

public class TimeSeriesHivesDataModel
{
    public required DateTime Timestamp { get; init; }
    public required float[] Values { get; init; }
}