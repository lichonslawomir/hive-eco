namespace Core.Domain.Aggregates;

public interface IEntity
{
    public uint Version { get; }
}

public interface IEntity<out TId> : IEntity
{
    TId Id { get; }
}

public abstract class Entity<TId> : IEntity<TId>
{
    public TId Id { get; protected set; } = default!;

    public uint Version { get; protected set; }

    protected Entity()
    {
    }

    protected Entity(TId id)
    {
        Id = id;
    }
}