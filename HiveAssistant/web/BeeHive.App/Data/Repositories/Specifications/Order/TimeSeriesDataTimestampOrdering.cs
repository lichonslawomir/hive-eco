using BeeHive.Domain.Data;
using Core.App.Repositories.Order;
using System.Linq.Expressions;

namespace BeeHive.App.Data.Repositories.Specifications.Order;

public class TimeSeriesDataTimestampOrdering : AOrder<TimeSeriesData, DateTime>
{
    public TimeSeriesDataTimestampOrdering(bool asc) : base(asc)
    {
    }

    public override Expression<Func<TimeSeriesData, DateTime>> OrderFunc => x => x.Timestamp;
}