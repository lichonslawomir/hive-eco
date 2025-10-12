using BeeHive.Domain.Aggregate.Events;
using BeeHive.Domain.Data;
using Core.Domain.Aggregates;

namespace BeeHive.Domain.Aggregate;

public class TimeAggregateSeries : AggregateRoot<int>
{
    public AggregationPeriod Period { get; private set; }
    public DateTime? LasteAggregateTime { get; private set; }

    public TimeSeries TimeSeries { get; private set; } = null!;
    public int TimeSeriesId { get; private set; }

    public IReadOnlyCollection<TimeAggregateSeriesData> Data { get; private set; } = new List<TimeAggregateSeriesData>();

    public IReadOnlyCollection<AudioAggregateStatsData> AudioStats { get; private set; } = new List<AudioAggregateStatsData>();

    public TimeAggregateSeries(TimeSeries timeSeries, AggregationPeriod period)
    {
        TimeSeries = timeSeries;
        Period = period;
    }

    protected TimeAggregateSeries(int id)
        : base(id)
    {
    }

    public TimeAggregateSeriesData CreateData(DateTime timestamp, int count,
        float? maxValue, float? minValue, float? avgValue, float? medValue)
    {
        var timeAggregateSeriesData = new TimeAggregateSeriesData()
        {
            TimeAggregateSeries = this,
            Timestamp = timestamp,
            Count = count,
            MaxValue = maxValue,
            MinValue = minValue,
            AvgValue = avgValue,
            MedValue = medValue
        };

        PublishEvent(new UpdateTimeAggregateSeriesDataEvent(this, timestamp));

        return timeAggregateSeriesData;
    }

    public void UpdateData(TimeAggregateSeriesData timeAggregateSeriesData, int count,
        float? maxValue, float? minValue, float? avgValue, float? medValue)
    {
        timeAggregateSeriesData.Count = count;
        timeAggregateSeriesData.MaxValue = maxValue;
        timeAggregateSeriesData.MinValue = minValue;
        timeAggregateSeriesData.AvgValue = avgValue;
        timeAggregateSeriesData.MedValue = medValue;

        PublishEvent(new UpdateTimeAggregateSeriesDataEvent(this, timeAggregateSeriesData.Timestamp));
    }

    public AudioAggregateStatsData CreateAudioStats(DateTime timestamp,
        float durationSec, float frequency, float amplitudePeak, float amplitudeRms, float amplitudeMav)
    {
        var audioAggregateStatsData = new AudioAggregateStatsData()
        {
            TimeAggregateSeries = this,
            Timestamp = timestamp,
            DurationSec = durationSec,
            Frequency = frequency,
            AmplitudePeak = amplitudePeak,
            AmplitudeRms = amplitudeRms,
            AmplitudeMav = amplitudeMav
        };

        PublishEvent(new UpdateAudioAggregateStatsDataEvent(this, timestamp));

        return audioAggregateStatsData;
    }

    public void UpdateAudioStats(AudioAggregateStatsData audioAggregateStatsData,
        float durationSec, float frequency, float amplitudePeak, float amplitudeRms, float amplitudeMav)
    {
        audioAggregateStatsData.DurationSec = durationSec;
        audioAggregateStatsData.Frequency = frequency;
        audioAggregateStatsData.AmplitudePeak = amplitudePeak;
        audioAggregateStatsData.AmplitudeRms = amplitudeRms;
        audioAggregateStatsData.AmplitudeMav = amplitudeMav;

        PublishEvent(new UpdateAudioAggregateStatsDataEvent(this, audioAggregateStatsData.Timestamp));
    }
}