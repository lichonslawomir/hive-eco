using BeeHive.Domain.Aggregate;
using Core.App.Repositories.Order;

namespace BeeHive.App.Aggregate.Repositories.Specifications.Order;

public class TimeAggregateSeriesDataTimestampAndHiveIdOrdering : IOrder<TimeAggregateSeriesData>
{
    public IOrderedQueryable<TimeAggregateSeriesData> ToOrderExpression(IQueryable<TimeAggregateSeriesData> entities)
    {
        return entities.OrderBy(x => x.Timestamp)
            .ThenBy(x => x.TimeAggregateSeries.TimeSeries.HiveId);
    }
}