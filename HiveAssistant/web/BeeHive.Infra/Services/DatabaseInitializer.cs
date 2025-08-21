using BeeHive.Infra.DataAccess.DbContexts;
using Core.Infra.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace BeeHive.Infra.Services;

internal sealed class DatabaseInitializer(BeeHiveDbContext dbContext) : IDatabaseInitializer
{
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        await dbContext.Database.MigrateAsync(cancellationToken);
    }
}