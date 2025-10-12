using BeeHive.App.Data.Repositories.Specifications.Filter;
using BeeHive.App.Data.Repositories.Specifications.Order;
using BeeHive.Contract.Data.Models;
using BeeHive.Domain.Data;
using Core.App.Repositories;
using Core.App.Repositories.Filter;
using Core.App.Repositories.Order;
using System.Linq.Expressions;

namespace BeeHive.App.Data.Repositories.Specifications;

public class TimeSeriesDataSpecification : IMapSpecification<TimeSeriesData, TimeSeriesDataModel>
{
    public int? HiveId { get; set; }

    public TimeSeriesKind? Kind { get; set; }

    public DateTime? From { get; set; }

    public DateTime? To { get; set; }

    public bool Asc { get; set; } = true;

    public bool Distinct => false;

    public IEnumerable<IFilter<TimeSeriesData>> AsEnumerableFilters()
    {
        if (HiveId.HasValue)
            yield return new TimeSeriesDataHiveIdFilter(HiveId.Value);

        if (Kind.HasValue)
            yield return new TimeSeriesDataKindFilter(Kind.Value);

        if (From.HasValue)
            yield return new TimeSeriesDataFromFilter(From.Value);

        if (To.HasValue)
            yield return new TimeSeriesDataToFilter(To.Value);
    }

    public IOrder<TimeSeriesData>? OrderBy()
    {
        return new TimeSeriesDataTimestampOrdering(Asc);
    }

    public Expression<Func<TimeSeriesData, TimeSeriesDataModel>> Selector => MappingExtensions.Map;
}