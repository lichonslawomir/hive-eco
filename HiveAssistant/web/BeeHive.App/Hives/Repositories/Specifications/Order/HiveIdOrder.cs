using BeeHive.Domain.Hives;
using Core.App.Repositories.Order;
using System.Linq.Expressions;

namespace BeeHive.App.Hives.Repositories.Specifications.Order;

public class HiveIdOrder : AOrder<Hive, int>
{
    public HiveIdOrder(bool asc) : base(asc)
    {
    }

    public override Expression<Func<Hive, int>> OrderFunc => x => x.Id;
}