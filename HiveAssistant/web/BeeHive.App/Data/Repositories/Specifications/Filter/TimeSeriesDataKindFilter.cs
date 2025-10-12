using BeeHive.Domain.Data;
using Core.App.Repositories.Filter;
using System.Linq.Expressions;

namespace BeeHive.App.Data.Repositories.Specifications.Filter;

public class TimeSeriesDataKindFilter(TimeSeriesKind kind) : IFilter<TimeSeriesData>
{
    public Expression<Func<TimeSeriesData, bool>> Filter => x => x.TimeSeries.Kind == kind;
}