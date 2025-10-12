namespace Core.Domain.Aggregates;

public interface ISynchronizableEntity
{
    DateTime CreatedOrUpdatedDate { get; }
}