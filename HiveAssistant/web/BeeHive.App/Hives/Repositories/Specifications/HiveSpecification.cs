using BeeHive.App.Hives.Repositories.Specifications.Filter;
using BeeHive.App.Hives.Repositories.Specifications.Order;
using BeeHive.Contract.Hives;
using BeeHive.Domain.Hives;
using Core.App.Repositories;
using Core.App.Repositories.Filter;
using Core.App.Repositories.Order;
using System.Linq.Expressions;

namespace BeeHive.App.Hives.Repositories.Specifications;

public class HiveSpecification : IMapSpecification<Hive, HiveDto>
{
    public (string Key, string HoldingKey)? BeeGarden;

    public bool Distinct => false;

    public IEnumerable<IFilter<Hive>>? AsEnumerableFilters()
    {
        if (BeeGarden.HasValue)
            yield return new HiveBeeGardenFilter(BeeGarden.Value.HoldingKey, BeeGarden.Value.Key);
        yield break;
    }

    public IOrder<Hive>? OrderBy()
    {
        return new HiveIdOrder(true);
    }

    public Expression<Func<Hive, HiveDto>> Selector()
    {
        return MappingExtensions.Map;
    }
}