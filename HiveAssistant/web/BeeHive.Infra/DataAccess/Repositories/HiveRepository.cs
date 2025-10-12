using BeeHive.App.Hives.Repositories;
using BeeHive.Domain.Hives;
using BeeHive.Infra.DataAccess.DbContexts;
using Core.Infra.DataAccess.Repositories;

namespace BeeHive.Infra.DataAccess.Repositories;

internal class HiveRepository : GenericRepository<Hive, int, BeeHiveDbContext>, IHiveRepository
{
    public HiveRepository(BeeHiveDbContext dbContext) : base(dbContext)
    {
    }
}