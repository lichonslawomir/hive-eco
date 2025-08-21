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

    protected BaseDbContext(DbContextOptions<TDbContext> options, IWorkContext workContext) : base(options)
    {
        _workContext = workContext;
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        OnBeforeSaveChanges(ChangeTracker.Entries().ToList());
        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        OnBeforeSaveChanges(ChangeTracker.Entries().ToList());
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    protected virtual void OnBeforeSaveChanges(IList<EntityEntry> entityEntries)
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
        if (entityEntry.Entity is not ICreationAudited<TUserId> entity)
            return;

        Entry(entity).Property(x => x.CreatedBy).CurrentValue = _userId ??= _workContext.GetUserId<TUserId>();
        Entry(entity).Property(x => x.CreatedDate).CurrentValue = _now ??= _workContext.Now();
    }

    private void SetUpdateAudit(EntityEntry entityEntry)
    {
        if (entityEntry.Entity is not IAuditableEntity<TUserId> entity)
            return;

        Entry(entity).Property(x => x.UpdatedBy).CurrentValue = _userId ??= _workContext.GetUserId<TUserId>();
        Entry(entity).Property(x => x.UpdatedDate).CurrentValue = _now ??= _workContext.Now();
    }
}