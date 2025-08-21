using BeeHive.Domain.Aggregate;
using Core.App.Repositories.Filter;
using System.Linq.Expressions;

namespace BeeHive.App.Aggregate.Repositories.Specifications.Filter;

public class TimeAggregateSeriesDataHiveIdFilter(int hiveId) : IFilter<TimeAggregateSeriesData>
{
    public Expression<Func<TimeAggregateSeriesData, bool>> Filter => x => x.TimeAggregateSeries.TimeSeries.HiveId == hiveId;
}