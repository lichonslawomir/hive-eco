namespace BeeHive.Domain.Aggregate;

public class TimeAggregateSeriesData
{
    internal TimeAggregateSeriesData()
    {
    }

    public TimeAggregateSeries TimeAggregateSeries { get; internal set; } = null!;
    public int TimeAggregateSeriesId { get; internal set; }

    public DateTime Timestamp { get; internal set; }

    public int Count { get; internal set; }
    public float? MaxValue { get; internal set; }
    public float? MinValue { get; internal set; }
    public float? AvgValue { get; internal set; }
    public float? MedValue { get; internal set; }
}