using BeeHive.Infra.DataAccess.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace BeeHive.Infra.Sqlite.Services;

public class BeeHiveDbContextConfigurationProvider : IBeeHiveDbContextConfigurationProvider
{
    public void ApplyConfiguration(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(BeeHiveDbContextConfigurationProvider).Assembly);
    }
}