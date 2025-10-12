using BeeHive.Domain.Data.Events;
using BeeHive.Domain.Hives;
using Core.Domain.Aggregates;

namespace BeeHive.Domain.Data;

public class TimeSeries : AggregateRoot<int>
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

    protected TimeSeries(int id)
        : base(id)
    {
    }

    public void AddData(IEnumerable<(DateTime timestamp, float vale)> data)
    {
        var count = _data.Count;
        _data.AddRange(data.Select(x => new TimeSeriesData(this, x.timestamp, x.vale)));

        PublishEvent(new AddTimeSeriesDataEvent(this, _data.Count - count));
    }
}