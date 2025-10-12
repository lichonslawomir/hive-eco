using BeeHive.App.Hives.Repositories.Specifications.Filter;
using BeeHive.App.Hives.Repositories.Specifications.Order;
using BeeHive.Domain.Hives;
using Core.App.Repositories;
using Core.App.Repositories.Filter;
using Core.App.Repositories.Order;

namespace BeeHive.App.Hives.Repositories.Specifications;

public enum HiveOrdering
{
    IdAsc,
    CreatedOrUpdatedDateAsc
}

public class HiveSpecification : ISpecification<Hive>
{
    public (string Key, string HoldingKey)? BeeGarden;

    public HiveOrdering Ordering = HiveOrdering.IdAsc;

    public IEnumerable<IFilter<Hive>> AsEnumerableFilters()
    {
        if (BeeGarden.HasValue)
            yield return new HiveBeeGardenFilter(BeeGarden.Value.HoldingKey, BeeGarden.Value.Key);

        yield break;
    }

    public IOrder<Hive>? OrderBy()
    {
        switch (Ordering)
        {
            case HiveOrdering.IdAsc:
                return new HiveIdOrder(true);

            case HiveOrdering.CreatedOrUpdatedDateAsc:
                return new CreatedOrUpdatedDateOrder<Hive>(true);

            default:
                throw new NotImplementedException($"{Ordering}");
        }
    }
}