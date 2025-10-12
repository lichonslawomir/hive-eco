using System.Linq.Expressions;
using Core.App.Repositories.Order;
using Core.Domain.Aggregates;

namespace BeeHive.App.Hives.Repositories.Specifications.Order;

public class CreatedOrUpdatedDateOrder<TSynchronizableEntity> : AOrder<TSynchronizableEntity, DateTime>
    where TSynchronizableEntity : class, ISynchronizableEntity
{
    public CreatedOrUpdatedDateOrder(bool asc) : base(asc)
    {
    }

    public override Expression<Func<TSynchronizableEntity, DateTime>> OrderFunc => x => x.CreatedOrUpdatedDate;
}