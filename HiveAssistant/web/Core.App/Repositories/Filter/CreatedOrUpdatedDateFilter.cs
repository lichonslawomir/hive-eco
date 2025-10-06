using Core.Domain.Aggregates;
using System.Linq.Expressions;

namespace Core.App.Repositories.Filter;

public class CreatedOrUpdatedDateFilter<TSynchronizableEntity>(DateTime lastDate) : IFilter<TSynchronizableEntity>
    where TSynchronizableEntity : class, ISynchronizableEntity
{
    public Expression<Func<TSynchronizableEntity, bool>> Filter => x => x.CreatedOrUpdatedDate > lastDate;
}