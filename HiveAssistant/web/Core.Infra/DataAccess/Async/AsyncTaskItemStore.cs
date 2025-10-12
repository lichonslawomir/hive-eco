using Core.App.Handlers.Async;
using Core.Domain.Aggregates;

namespace Core.Infra.DataAccess.Async;

internal class AsyncTaskItemStore : IEntity<int>
{
    public int Id { get; set; }
    public DateTimeOffset CreationDate { get; set; }
    public AsyncTaskItem? Item { get; set; }

    public DateTimeOffset? LockUntil { get; set; }

    public uint Version { get; set; }
}