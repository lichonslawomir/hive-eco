using BeeHive.Infra.DataAccess.DbContexts;
using Core.Infra.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BeeHive.Infra.Sqlite.Services;

internal sealed class DatabaseInitializer(BeeHiveDbContext dbContext, ILogger<DatabaseInitializer> logger) : IDatabaseInitializer
{
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation($"Check {nameof(DatabaseInitializer)}");
        var pendings = await dbContext.Database.GetPendingMigrationsAsync();
        logger.LogInformation($"Pendings {string.Join(";", pendings)}");
        await dbContext.Database.MigrateAsync(cancellationToken);
    }
}