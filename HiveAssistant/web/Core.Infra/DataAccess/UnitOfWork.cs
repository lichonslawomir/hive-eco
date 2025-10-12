using Core.App;
using Core.App.DataAccess;
using Core.App.Extensions;
using Core.App.Handlers;
using Core.Contract.Executers;
using Core.Domain.Aggregates;
using Core.Domain.DomainEvents;
using Core.Infra.DataAccess.DbContexts;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Core.Infra.DataAccess;

internal sealed class UnitOfWork<TDbContext, TUserId>(
    TDbContext dbContext,
    ICommandBus commandBus,
    IHandlerProvider handlerProvider) : IUnitOfWork
    where TDbContext : BaseDbContext<TDbContext, TUserId>
{
    private List<IDomainEvent>? _domainEvents;

    public event CommitDelegate? CommitEvent;

    public event ResetCommitDelegate? ResetEvent;

    public async Task<ICommitResult> CommitAsync(CancellationToken cancellationToken)
    {
        var domainEvents = GetAllDomainEvents();

        foreach (var domainEvent in domainEvents)
        {
            await commandBus.Publish(domainEvent, cancellationToken);
        }

        if (_domainEvents is null)
            _domainEvents = domainEvents;
        else
            _domainEvents.AddRange(domainEvents);

        var saveResult = await dbContext.SaveChangesAsync(cancellationToken);

        await CommitEvent.InvokeAsync(cancellationToken);

        return new CommitResult(_domainEvents);
    }

    public void Reset()
    {
        dbContext.ChangeTracker.Clear();
        handlerProvider.Reset();
    }

    private List<IDomainEvent> GetAllDomainEvents()
    {
        var domainEntities = GetDomainEntities();

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        foreach (var entity in domainEntities)
        {
            entity.Entity.ClearAllDomainEvents();
        }

        return domainEvents;
    }

    private IList<EntityEntry<IAggregateRoot>> GetDomainEntities()
    {
        return dbContext.ChangeTracker
            .Entries<IAggregateRoot>()
            .ToList();
    }
}