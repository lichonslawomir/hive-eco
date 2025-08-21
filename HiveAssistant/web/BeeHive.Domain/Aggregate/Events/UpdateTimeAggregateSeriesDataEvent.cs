using Core.Domain.DomainEvents;
using System.Text.Json.Serialization;

namespace BeeHive.Domain.Aggregate.Events;

public class UpdateTimeAggregateSeriesDataEvent : IDomainEvent<int>
{
    protected internal readonly TimeAggregateSeries? _entity;
    private readonly int? _entityId;

    [JsonConstructor]
    public UpdateTimeAggregateSeriesDataEvent(int entityId, AggregationPeriod period, DateTime timestamp)
    {
        _entityId = entityId;
        Period = period;
        Timestamp = timestamp;
    }

    public UpdateTimeAggregateSeriesDataEvent(TimeAggregateSeries entity, DateTime timestamp)
    {
        _entity = entity;
        Period = entity.Period;
        Timestamp = timestamp;
    }

    public int EntityId
    {
        get
        {
            if (_entity is not null)
                return _entity.Id;
            return _entityId ?? throw new NotSupportedException("Id not found");
        }
    }

    public AggregationPeriod Period { get; private set; }
    public DateTime Timestamp { get; private set; }
}