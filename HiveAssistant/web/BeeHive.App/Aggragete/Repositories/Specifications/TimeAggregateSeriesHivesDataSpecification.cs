using System.Linq.Expressions;
using BeeHive.App.Aggregate.Repositories.Specifications.Filter;
using BeeHive.App.Aggregate.Repositories.Specifications.Order;
using BeeHive.App.Hives.Repositories.Specifications;
using BeeHive.App.Hives.Repositories.Specifications.Order;
using BeeHive.Contract.Aggregate.Models;
using BeeHive.Domain.Aggregate;
using BeeHive.Domain.Data;
using Core.App.Repositories;
using Core.App.Repositories.Filter;
using Core.App.Repositories.Order;

namespace BeeHive.App.Aggregate.Repositories.Specifications;

public enum TimeAggregateSeriesHivesDataOrdering
{
    IdAsc,
    CreatedOrUpdatedDateAsc
}

public class TimeAggregateSeriesHivesDataSpecification : IMapSpecification<TimeAggregateSeriesData, TimeAggregateSeriesDataModelEx>
{
    public int[]? HiveIds { get; set; }

    public TimeSeriesKind? Kind { get; set; }

    public AggregationPeriod? Period { get; set; }

    public DateTime? From { get; set; }

    public DateTime? To { get; set; }

    public DateTime? CreatedOrUpdatedDate { get; set; }

    public HiveOrdering Ordering = HiveOrdering.IdAsc;

    public bool Distinct => false;

    public IEnumerable<IFilter<TimeAggregateSeriesData>> AsEnumerableFilters()
    {
        if (HiveIds is not null)
            yield return new TimeAggregateSeriesDataHiveIdsFilter(HiveIds);

        if (Kind.HasValue)
            yield return new TimeAggregateSeriesDataKindFilter(Kind.Value);

        if (Period.HasValue)
            yield return new TimeAggregateSeriesDataPeriodFilter(Period.Value);

        if (From.HasValue)
            yield return new TimeAggregateSeriesDataFromFilter(From.Value);

        if (To.HasValue)
            yield return new TimeAggregateSeriesDataToFilter(To.Value);

        if (CreatedOrUpdatedDate.HasValue)
            yield return new CreatedOrUpdatedDateFilter<TimeAggregateSeriesData>(CreatedOrUpdatedDate.Value);
    }

    public IOrder<TimeAggregateSeriesData>? OrderBy()
    {
        switch (Ordering)
        {
            case HiveOrdering.IdAsc:
                return new TimeAggregateSeriesDataTimestampAndHiveIdOrdering();

            case HiveOrdering.CreatedOrUpdatedDateAsc:
                return new CreatedOrUpdatedDateOrder<TimeAggregateSeriesData>(true);

            default:
                throw new NotImplementedException($"{Ordering}");
        }
    }

    public Expression<Func<TimeAggregateSeriesData, TimeAggregateSeriesDataModelEx>> Selector => MappingExtensions.MapEx;
}