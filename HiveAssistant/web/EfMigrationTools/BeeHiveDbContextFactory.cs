using System.Text.Json;
using BeeHive.Infra.DataAccess.DbContexts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EfMigrationTools;

public class BeeHiveDbContextFactory : DesignTimeDbContextFactory<BeeHiveDbContext>
{
    protected override void AddDbContext(IServiceCollection services, IConfiguration configuration)
    {
        string? dbType = configuration["DbType"];
        Console.WriteLine($"db-type: {dbType}");
        if (dbType == "Sqlite")
            BeeHive.Infra.Sqlite.Extensions.DependencyInjection.ServiceCollectionExtensions.AddBeeHiveDbContext(services, configuration);
        if (dbType == "Postgres")
            BeeHive.Infra.Postgres.Extensions.DependencyInjection.ServiceCollectionExtensions.AddBeeHiveDbContext(services, configuration);
    }
}