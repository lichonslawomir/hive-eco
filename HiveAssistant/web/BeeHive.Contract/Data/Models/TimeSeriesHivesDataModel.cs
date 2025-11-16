namespace BeeHive.Contract.Data.Models;

public struct TimeSeriesHivesDataModel
{
    public required DateTimeOffset Timestamp { get; init; }
    public float[] Values { get; init; }
}