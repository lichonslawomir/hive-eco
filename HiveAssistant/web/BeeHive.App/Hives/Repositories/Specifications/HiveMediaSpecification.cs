using BeeHive.App.Hives.Repositories.Specifications.Filter;
using BeeHive.App.Hives.Repositories.Specifications.Order;
using BeeHive.Domain.Hives;
using Core.App.Repositories;
using Core.App.Repositories.Filter;
using Core.App.Repositories.Order;

namespace BeeHive.App.Hives.Repositories.Specifications;

public enum HiveMediaOrdering
{
    IdAsc,
    CreatedOrUpdatedDateAsc
}

public class HiveMediaSpecification : ISpecification<HiveMedia>
{
    public int? HiveId;

    public HiveOrdering Ordering = HiveOrdering.IdAsc;

    public IEnumerable<IFilter<HiveMedia>> AsEnumerableFilters()
    {
        if (HiveId.HasValue)
            yield return new HiveMediaFilter(HiveId.Value);
        yield break;
    }

    public IOrder<HiveMedia>? OrderBy()
    {
        switch (Ordering)
        {
            case HiveOrdering.IdAsc:
                return new HiveMediaIdOrder(true);

            case HiveOrdering.CreatedOrUpdatedDateAsc:
                return new CreatedOrUpdatedDateOrder<HiveMedia>(true);

            default:
                throw new NotImplementedException($"{Ordering}");
        }
    }
}