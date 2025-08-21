using BeeHive.Domain.Data;
using Core.App.Repositories.Order;

namespace BeeHive.App.Data.Repositories.Specifications.Order;

public class TimeSeriesDataTimestampAndHiveIdOrdering : IOrder<TimeSeriesData>
{
    public IOrderedQueryable<TimeSeriesData> ToOrderExpression(IQueryable<TimeSeriesData> entities)
    {
        return entities.OrderBy(x => x.Timestamp)
            .ThenBy(x => x.TimeSeries.HiveId);
    }
}