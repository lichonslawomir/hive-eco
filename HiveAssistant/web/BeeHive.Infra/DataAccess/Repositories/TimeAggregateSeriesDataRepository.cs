using BeeHive.App.Aggregate.Repositories;
using BeeHive.Domain.Aggregate;
using BeeHive.Infra.DataAccess.DbContexts;
using Core.Infra.DataAccess.Repositories;

namespace BeeHive.Infra.DataAccess.Repositories;

internal class TimeAggregateSeriesDataRepository : GenericRepository<TimeAggregateSeriesData, BeeHiveDbContext>, ITimeAggregateSeriesDataRepository
{
    public TimeAggregateSeriesDataRepository(BeeHiveDbContext dbContext) : base(dbContext)
    {
    }
}