using Core.App.Handlers.Async;
using Microsoft.Extensions.Logging;

namespace Core.Infra.DataAccess.Async;

internal class InMemoryAsyncTaskRepository : IAsyncTaskRepository
{
    private readonly List<AsyncTaskItemStore> _scopeListToAdd = new List<AsyncTaskItemStore>();
    private readonly List<(int lvl, AsyncTaskItemStore item)> _scopeListToDel = new List<(int lvl, AsyncTaskItemStore item)>();
    private readonly AsyncTaskSharedStore _store;

    private readonly ILogger<InMemoryAsyncTaskRepository> _logger;

    public InMemoryAsyncTaskRepository(AsyncTaskSharedStore store, ILogger<InMemoryAsyncTaskRepository> logger)
    {
        _store = store;
        _logger = logger;
    }

    public ValueTask<string[]> GetQueueIds()
    {
        return _store.GetQueueIds();
    }

    public async ValueTask<(AsyncTaskItem? item, int? waitMsTime)> Pop(string queueId)
    {
        var popRecord = await _store.Pop(queueId);
        if (popRecord.HasValue)
        {
            var (record, lvl) = popRecord.Value;
            if (record.LockUntil.HasValue)
            {
                var now = DateTime.UtcNow;
                if (record.LockUntil > now)
                {
                    return (null, (int)(record.LockUntil.Value - now).TotalMilliseconds);
                }
            }
            _scopeListToDel.Add((lvl, record));
            return (record.Item, null);
        }
        return (null, null);
    }

    public async ValueTask SaveHandleItems()
    {
        await _store.Update(null, _scopeListToDel);
        _scopeListToDel.Clear();
    }

    public ValueTask SetHandleError(AsyncTaskItem item, string? errorMessage)
    {//Ignore
        return ValueTask.CompletedTask;
    }

    public async ValueTask SetHandleException(AsyncTaskItem item, Exception ex, bool outOfHandler)
    {
        var itemToDel = _scopeListToDel.FirstOrDefault(i => i.item.Item == item);
        if (itemToDel.item != null)
        {//level up item and lock it for some time
            _scopeListToDel.Remove(itemToDel);
            await _store.LevelUp(itemToDel.item, itemToDel.lvl, _logger);
        }
    }

    public ValueTask Push(AsyncTaskItem item)
    {
        var storeItem = new AsyncTaskItemStore()
        {
            Item = item
        };
        _scopeListToAdd.Add(storeItem);
        return ValueTask.CompletedTask;
    }

    public async ValueTask SavePushedItems()
    {
        await _store.Update(_scopeListToAdd, null);
        _scopeListToAdd.Clear();
    }

    public void ClearPushedItems()
    {
        _scopeListToAdd.Clear();
    }
}