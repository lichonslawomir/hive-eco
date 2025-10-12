using BeeHive.Domain.Aggregate;
using Core.App.Repositories.Order;
using System.Linq.Expressions;

namespace BeeHive.App.Aggregate.Repositories.Specifications.Order;

public class TimeAggregateSeriesDataTimestampOrdering : AOrder<TimeAggregateSeriesData, DateTime>
{
    public TimeAggregateSeriesDataTimestampOrdering(bool asc) : base(asc)
    {
    }

    public override Expression<Func<TimeAggregateSeriesData, DateTime>> OrderFunc => x => x.Timestamp;
}