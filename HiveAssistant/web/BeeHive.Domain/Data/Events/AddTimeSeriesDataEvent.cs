using Core.Domain.DomainEvents;
using System.Text.Json.Serialization;

namespace BeeHive.Domain.Data.Events;

public class AddTimeSeriesDataEvent : IDomainEvent<int>
{
    protected internal readonly TimeSeries? _entity;
    private readonly int? _entityId;

    [JsonConstructor]
    public AddTimeSeriesDataEvent(int entityId, int count)
    {
        _entityId = entityId;
        Count = count;
    }

    public AddTimeSeriesDataEvent(TimeSeries entity, int count)
    {
        _entity = entity;
        Count = count;
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

    public int Count { get; private set; }
}