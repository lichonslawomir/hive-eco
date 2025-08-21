using Core.App;
using Core.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Core.Infra.DataAccess.DbContexts;

public abstract class BaseDbContextManuallyVersioned<TDbContext, TUserId> : BaseDbContext<TDbContext, TUserId> where TDbContext : DbContext
{
    protected BaseDbContextManuallyVersioned(DbContextOptions<TDbContext> options, IWorkContext workContext) : base(options, workContext)
    {
    }

    protected override void OnBeforeSaveChanges(IList<EntityEntry> entityEntries)
    {
        base.OnBeforeSaveChanges(entityEntries);
        foreach (var entityEntry in entityEntries)
        {
            switch (entityEntry.State)
            {
                case EntityState.Added:
                    SetVersion(entityEntry);
                    break;

                case EntityState.Modified:
                    UpdateVersion(entityEntry);
                    break;
            }
        }
    }

    private void SetVersion(EntityEntry entityEntry)
    {
        if (entityEntry.Entity is not IEntity entity)
            return;

        Entry(entity).Property(x => x.Version).CurrentValue = 1;
    }

    private void UpdateVersion(EntityEntry entityEntry)
    {
        if (entityEntry.Entity is not IEntity entity)
            return;

        Entry(entity).Property(x => x.Version).CurrentValue++;
    }
}