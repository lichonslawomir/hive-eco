using BeeHive.Domain.Aggregate;
using Core.App.Repositories;

namespace BeeHive.App.Aggregate.Repositories;

public interface ITimeAggregateSeriesDataRepository : IGenericRepository<TimeAggregateSeriesData>
{
}