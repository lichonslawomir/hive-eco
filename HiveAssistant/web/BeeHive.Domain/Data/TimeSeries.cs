using BeeHive.Domain.Data.Events;
using BeeHive.Domain.Hives;
using Core.Domain.Aggregates;

namespace BeeHive.Domain.Data;

public sealed class TimeSeries : AggregateRoot<int>
{
    private readonly List<TimeSeriesData> _data = new();

    public TimeSeriesKind Kind { get; private set; }

    public Hive Hive { get; private set; } = null!;
    public int HiveId { get; private set; }

    public IReadOnlyCollection<TimeSeriesData> Data { get => _data; }

    public TimeSeries(Hive hive, TimeSeriesKind kind)
    {
        Hive = hive;
        Kind = kind;
    }

    private TimeSeries(int id)
        : base(id)
    {
    }

    public void AddData(IEnumerable<(DateTime timestamp, float vale)> data)
    {
        var count = _data.Count;
        _data.AddRange(data.Select(x => new TimeSeriesData(this, x.timestamp, x.vale)));

        PublishEvent(new AddTimeSeriesDataEvent(this, _data.Count - count));
    }

    public TimeSeriesData AddData(DateTime timestamp, float vale)
    {
        var d = new TimeSeriesData(this, timestamp, vale);
        _data.Add(d);

        PublishEvent(new AddTimeSeriesDataEvent(this, 1));
        return d;
    }

    public void UpdateData(TimeSeriesData timeSeriesData, float vale)
    {
        if (timeSeriesData.Value != vale)
        {
            timeSeriesData.Value = vale;
            PublishEvent(new AddTimeSeriesDataEvent(this, 1));
        }
    }
}