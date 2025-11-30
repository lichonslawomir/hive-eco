using BeeHive.Infra.DataAccess.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace BeeHive.Infra.Postgres.Services;

public class BeeHiveDbContextConfigurationProvider : IBeeHiveDbContextConfigurationProvider
{
    public void ApplyConfiguration(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(BeeHiveDbContextConfigurationProvider).Assembly);
    }
}