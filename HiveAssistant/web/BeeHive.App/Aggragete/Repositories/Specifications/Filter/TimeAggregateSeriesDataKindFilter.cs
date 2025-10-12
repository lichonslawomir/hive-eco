using BeeHive.Domain.Aggregate;
using BeeHive.Domain.Data;
using Core.App.Repositories.Filter;
using System.Linq.Expressions;

namespace BeeHive.App.Aggregate.Repositories.Specifications.Filter;

public class TimeAggregateSeriesDataKindFilter(TimeSeriesKind kind) : IFilter<TimeAggregateSeriesData>
{
    public Expression<Func<TimeAggregateSeriesData, bool>> Filter => x => x.TimeAggregateSeries.TimeSeries.Kind == kind;
}