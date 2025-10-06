using Core.App;
using Core.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Core.Infra.DataAccess.DbContexts;

public abstract class BaseDbContext<TDbContext, TUserId> : DbContext where TDbContext : DbContext
{
    private readonly IWorkContext _workContext;

    private DateTime? _now;
    private TUserId? _userId;

    protected BaseDbContext(DbContextOptions<TDbContext> options,
        IWorkContext workContext) : base(options)
    {
        _workContext = workContext;
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        OnBeforeSaveChanges(ChangeTracker.Entries());
        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        OnBeforeSaveChanges(ChangeTracker.Entries());
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    protected virtual void OnBeforeSaveChanges(IEnumerable<EntityEntry> entityEntries)
    {
        foreach (var entityEntry in entityEntries)
        {
            switch (entityEntry.State)
            {
                case EntityState.Added:
                    SetCreationAudit(entityEntry);
                    break;

                case EntityState.Modified:
                    SetUpdateAudit(entityEntry);
                    break;
            }
        }
    }

    private void SetCreationAudit(EntityEntry entityEntry)
    {
        if (entityEntry.Entity is ICreationAudited<TUserId> entity)
        {
            entityEntry.Property(nameof(ICreationAudited<TUserId>.CreatedBy)).CurrentValue = _userId ??= _workContext.GetUserId<TUserId>();
            //((EntityEntry<ICreationAudited<TUserId>>)entityEntry).Property(x => x.CreatedBy).CurrentValue = _userId ??= _workContext.GetUserId<TUserId>();
            Entry(entity).Property(x => x.CreatedDate).CurrentValue = _now ??= _workContext.Now();
        }

        if (entityEntry.Entity is ISynchronizableEntity synchronizableEntity)
        {
            var e = Entry(synchronizableEntity);
            var p1 = e.Property(x => x.CreatedOrUpdatedDate);
            if (!p1.IsModified)
                p1.CurrentValue = _now ??= _workContext.Now();
        }
    }

    private void SetUpdateAudit(EntityEntry entityEntry)
    {
        if (entityEntry.Entity is IAuditableEntity<TUserId> entity)
        {
            var e = Entry(entity);
            var p1 = e.Property(x => x.UpdatedBy);
            if (!p1.IsModified)
                p1.CurrentValue = _userId ??= _workContext.GetUserId<TUserId>();
            var p2 = e.Property(x => x.UpdatedDate);
            if (!p2.IsModified)
                p2.CurrentValue = _now ??= _workContext.Now();
        }

        if (entityEntry.Entity is ISynchronizableEntity synchronizableEntity)
        {
            var e = Entry(synchronizableEntity);
            var p1 = e.Property(x => x.CreatedOrUpdatedDate);
            if (!p1.IsModified)
                p1.CurrentValue = _now ??= _workContext.Now();
        }
    }
}