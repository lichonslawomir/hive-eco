using BeeHive.Domain.Data;
using Core.App.Repositories.Filter;
using System.Linq.Expressions;

namespace BeeHive.App.Data.Repositories.Specifications.Filter;

public class TimeSeriesDataHiveIdFilter(int hiveId) : IFilter<TimeSeriesData>
{
    public Expression<Func<TimeSeriesData, bool>> Filter => x => x.TimeSeries.HiveId == hiveId;
}