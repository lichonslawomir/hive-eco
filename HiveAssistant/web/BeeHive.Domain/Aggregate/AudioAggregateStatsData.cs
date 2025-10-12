namespace BeeHive.Domain.Aggregate;

public sealed class AudioAggregateStatsData
{
    internal AudioAggregateStatsData()
    {
    }

    public TimeAggregateSeries TimeAggregateSeries { get; internal set; } = null!;
    public int TimeAggregateSeriesId { get; internal set; }

    public DateTime Timestamp { get; internal set; }

    public float DurationSec { get; internal set; }
    public float Frequency { get; internal set; }
    public float AmplitudePeak { get; internal set; }
    public float AmplitudeRms { get; internal set; }
    public float AmplitudeMav { get; internal set; }
}