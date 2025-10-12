using Core.Domain.Aggregates;
using System.Text.Json.Serialization;

namespace Core.Domain.DomainEvents;

public interface INewEntityDomainEvent<out TId> : IDomainEvent<TId>
{
}

public abstract class NewEntityDomainEvent<TId> : INewEntityDomainEvent<TId>
{
    protected internal readonly IEntity<TId>? _entity;
    private readonly TId? _entityId;

    [JsonConstructor]
    public NewEntityDomainEvent(TId entityId)
    {
        _entityId = entityId;
    }

    public NewEntityDomainEvent(IEntity<TId> entity)
    {
        _entity = entity;
    }

    public TId EntityId
    {
        get
        {
            if (_entity is not null)
                return _entity!.Id ?? throw new NotSupportedException("Id not found");
            return _entityId ?? throw new NotSupportedException("Id not found");
        }
    }
}

public abstract class NewEntityDomainEvent<TEntity, TId> : NewEntityDomainEvent<TId>
    where TEntity : class, IEntity<TId>
{
    [JsonConstructor]
    public NewEntityDomainEvent(TId id) : base(id)
    {
    }

    public NewEntityDomainEvent(TEntity entity) : base(entity)
    {
    }

    public TEntity? GetEntity()
    {
        return _entity as TEntity;
    }
}