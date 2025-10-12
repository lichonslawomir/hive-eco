using Core.Contract.Executers;
using Core.Domain.DomainEvents;

namespace Core.Infra.DataAccess;

internal class CommitResult(IReadOnlyCollection<IDomainEvent> domainEvents) : ICommitResult
{
    public IEnumerable<TDomainEvent> GetDomainEvents<TDomainEvent>()
        where TDomainEvent : IDomainEvent
    {
        return domainEvents.OfType<TDomainEvent>();
    }
}