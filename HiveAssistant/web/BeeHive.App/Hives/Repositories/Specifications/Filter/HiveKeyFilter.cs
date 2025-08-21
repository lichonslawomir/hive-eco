using BeeHive.Domain.Hives;
using Core.App.Repositories.Filter;
using System.Linq.Expressions;

namespace BeeHive.App.Hives.Repositories.Specifications.Filter;

public class HiveKeyFilter(string key) : IFilter<Hive>
{
    public Expression<Func<Hive, bool>> Filter => x => x.UniqueKey == key;
}