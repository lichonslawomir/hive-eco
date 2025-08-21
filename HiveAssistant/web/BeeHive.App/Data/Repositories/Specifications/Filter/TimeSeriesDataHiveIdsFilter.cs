using BeeHive.Domain.Data;
using Core.App.Repositories.Filter;
using System.Linq.Expressions;

namespace BeeHive.App.Data.Repositories.Specifications.Filter;

public class TimeSeriesDataHiveIdsFilter(int[] hiveIds) : IFilter<TimeSeriesData>
{
    public Expression<Func<TimeSeriesData, bool>> Filter => x => hiveIds.Contains(x.TimeSeries.HiveId);
}