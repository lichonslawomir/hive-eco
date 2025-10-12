using BeeHive.App.Aggregate.Repositories.Specifications.Filter;
using BeeHive.App.Aggregate.Repositories.Specifications.Order;
using BeeHive.Contract.Aggregate.Models;
using BeeHive.Domain.Aggregate;
using BeeHive.Domain.Data;
using Core.App.Repositories;
using Core.App.Repositories.Filter;
using Core.App.Repositories.Order;
using System.Linq.Expressions;

namespace BeeHive.App.Aggregate.Repositories.Specifications;

public class TimeAggregateSeriesDataSpecification : IMapSpecification<TimeAggregateSeriesData, TimeAggregateSeriesDataModel>
{
    public int? HiveId { get; set; }

    public TimeSeriesKind? Kind { get; set; }

    public AggregationPeriod? Period { get; set; }

    public DateTime? From { get; set; }

    public DateTime? To { get; set; }

    public bool Distinct => false;

    public IEnumerable<IFilter<TimeAggregateSeriesData>>? AsEnumerableFilters()
    {
        if (HiveId.HasValue)
            yield return new TimeAggregateSeriesDataHiveIdFilter(HiveId.Value);

        if (Kind.HasValue)
            yield return new TimeAggregateSeriesDataKindFilter(Kind.Value);

        if (Period.HasValue)
            yield return new TimeAggregateSeriesDataPeriodFilter(Period.Value);

        if (From.HasValue)
            yield return new TimeAggregateSeriesDataFromFilter(From.Value);

        if (To.HasValue)
            yield return new TimeAggregateSeriesDataToFilter(To.Value);
    }

    public IOrder<TimeAggregateSeriesData>? OrderBy()
    {
        return new TimeAggregateSeriesDataTimestampOrdering(true);
    }

    public Expression<Func<TimeAggregateSeriesData, TimeAggregateSeriesDataModel>> Selector()
    {
        return MappingExtensions.Map;
    }
}