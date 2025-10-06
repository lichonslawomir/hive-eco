using BeeHive.App.Hives.Repositories;
using BeeHive.Domain.Hives;
using BeeHive.Infra.DataAccess.DbContexts;
using Core.Infra.DataAccess.Repositories;

namespace BeeHive.Infra.DataAccess.Repositories;

internal class HiveMediaRepository : GenericRepository<HiveMedia, int, BeeHiveDbContext>, IHiveMediaRepository
{
    public HiveMediaRepository(BeeHiveDbContext dbContext) : base(dbContext)
    {
    }
}