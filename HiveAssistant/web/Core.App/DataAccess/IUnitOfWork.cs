using Core.Contract.Executers;
using Core.Domain.DomainEvents;

namespace Core.App.DataAccess;

public delegate ValueTask CommitDelegate(CancellationToken cancellationToken);

public delegate void ResetCommitDelegate();

public interface IUnitOfWork
{
    event CommitDelegate? CommitEvent;

    event ResetCommitDelegate? ResetEvent;

    Task<ICommitResult> CommitAsync(CancellationToken cancellationToken);

    void Reset();
}

public static class CommandResultExtensions
{
    public static TDomainEvent GetDomainEvent<TDomainEvent>(this ICommitResult commitResult) where TDomainEvent : IDomainEvent
    {
        var domainEvent = commitResult.GetDomainEvents<TDomainEvent>().FirstOrDefault();
        if (domainEvent == null)
            throw new NotSupportedException($"Domain event {typeof(TDomainEvent).Name} not have been invoked");
        return domainEvent;
    }
}