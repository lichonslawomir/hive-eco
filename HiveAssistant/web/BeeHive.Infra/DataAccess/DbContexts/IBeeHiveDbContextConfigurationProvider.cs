using Microsoft.EntityFrameworkCore;

namespace BeeHive.Infra.DataAccess.DbContexts;

public interface IBeeHiveDbContextConfigurationProvider
{
    public void ApplyConfiguration(ModelBuilder builder);
}