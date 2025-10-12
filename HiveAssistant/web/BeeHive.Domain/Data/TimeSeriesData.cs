namespace BeeHive.Domain.Data;

public class TimeSeriesData
{
    internal TimeSeriesData(TimeSeries timeSeries, DateTime timestamp, float value)
    {
        TimeSeries = timeSeries;
        Timestamp = timestamp;
        Value = value;
    }

    private TimeSeriesData()
    {
    }

    public TimeSeries TimeSeries { get; private set; } = null!;
    public int TimeSeriesId { get; private set; }

    public DateTime Timestamp { get; private set; }

    public float Value { get; private set; }
}