using BeeHive.Domain.Hives;
using Core.App.Repositories.Filter;
using System.Linq.Expressions;

namespace BeeHive.App.Hives.Repositories.Specifications.Filter;

public class HiveBeeGardenFilter(string holdingKey, string beeGardenKey) : IFilter<Hive>
{
    public Expression<Func<Hive, bool>> Filter => x => x.BeeGarden.Holding.UniqueKey == holdingKey && x.BeeGarden.UniqueKey == beeGardenKey;
}