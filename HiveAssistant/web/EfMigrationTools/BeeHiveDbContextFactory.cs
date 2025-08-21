using BeeHive.Infra.DataAccess.DbContexts;
using BeeHive.Infra.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EfMigrationTools;

public class BeeHiveDbContextFactory : DesignTimeDbContextFactory<BeeHiveDbContext>
{
    protected override void AddDbContext(IServiceCollection services, IConfiguration configuration)
    {
        services.AddBeeHiveDbContext(configuration);
    }
}