using BeeHive.Domain.Hives;
using Core.App.Repositories.Filter;
using System.Linq.Expressions;

namespace BeeHive.App.Hives.Repositories.Specifications.Filter;

public class HiveMediaFilter(int hiveId) : IFilter<HiveMedia>
{
    public Expression<Func<HiveMedia, bool>> Filter => x => x.Hive.Id == hiveId;
}