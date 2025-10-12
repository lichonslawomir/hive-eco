using BeeHive.App.Data.Repositories;
using BeeHive.Domain.Data;
using BeeHive.Infra.DataAccess.DbContexts;
using Core.Infra.DataAccess.Repositories;

namespace BeeHive.Infra.DataAccess.Repositories;

internal class TimeSeriesDataRepository : GenericRepository<TimeSeriesData, BeeHiveDbContext>, ITimeSeriesDataRepository
{
    public TimeSeriesDataRepository(BeeHiveDbContext dbContext) : base(dbContext)
    {
    }
}