using Core.Domain.Aggregates;
using System.Text.Json.Serialization;

namespace Core.Domain.DomainEvents;

public interface IUpdatedEntityDomainEventt<out TId> : IDomainEvent<TId>
{
}

public abstract class UpdatedEntityDomainEvent<TId> : IUpdatedEntityDomainEventt<TId>
{
    private readonly TId? _entityId;

    [JsonConstructor]
    public UpdatedEntityDomainEvent(TId id)
    {
        _entityId = id;
    }

    public UpdatedEntityDomainEvent(IEntity<TId> entity)
    {
        _entityId = entity.Id;
    }

    public TId EntityId
    {
        get
        {
            return _entityId ?? throw new NotSupportedException("Id not initialized");
        }
    }
}