using BeeHive.Domain.Aggregate;
using Core.App.Repositories.Filter;
using System.Linq.Expressions;

namespace BeeHive.App.Aggregate.Repositories.Specifications.Filter;

public class TimeAggregateSeriesDataPeriodFilter(AggregationPeriod period) : IFilter<TimeAggregateSeriesData>
{
    public Expression<Func<TimeAggregateSeriesData, bool>> Filter => x => x.TimeAggregateSeries.Period == period;
}