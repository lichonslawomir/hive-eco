using BeeHive.Domain.Aggregate;
using Core.App.Repositories.Filter;
using System.Linq.Expressions;

namespace BeeHive.App.Aggregate.Repositories.Specifications.Filter;

public class TimeAggregateSeriesDataToFilter(DateTime date) : IFilter<TimeAggregateSeriesData>
{
    public Expression<Func<TimeAggregateSeriesData, bool>> Filter => x => x.Timestamp <= date;
}