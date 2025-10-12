using Core.Domain.DomainEvents;

namespace Core.Contract.Executers;

public interface ICommitResult
{
    public IEnumerable<TDomainEvent> GetDomainEvents<TDomainEvent>() where TDomainEvent : IDomainEvent;
}
