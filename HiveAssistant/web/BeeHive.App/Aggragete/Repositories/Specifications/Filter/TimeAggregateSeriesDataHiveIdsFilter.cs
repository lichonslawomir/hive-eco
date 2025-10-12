using BeeHive.Domain.Aggregate;
using Core.App.Repositories.Filter;
using System.Linq.Expressions;

namespace BeeHive.App.Aggregate.Repositories.Specifications.Filter;

public class TimeAggregateSeriesDataHiveIdsFilter(int[] hiveIds) : IFilter<TimeAggregateSeriesData>
{
    public Expression<Func<TimeAggregateSeriesData, bool>> Filter => x => hiveIds.Contains(x.TimeAggregateSeries.TimeSeries.HiveId);
}