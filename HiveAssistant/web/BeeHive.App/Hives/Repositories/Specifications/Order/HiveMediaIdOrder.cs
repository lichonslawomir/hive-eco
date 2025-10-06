using BeeHive.Domain.Hives;
using Core.App.Repositories.Order;
using System.Linq.Expressions;

namespace BeeHive.App.Hives.Repositories.Specifications.Order;

public class HiveMediaIdOrder : AOrder<HiveMedia, int>
{
    public HiveMediaIdOrder(bool asc) : base(asc)
    {
    }

    public override Expression<Func<HiveMedia, int>> OrderFunc => x => x.Id;
}