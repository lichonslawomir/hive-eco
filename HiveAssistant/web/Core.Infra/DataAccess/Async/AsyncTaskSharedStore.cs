using Core.App.Handlers.Async;
using Microsoft.Extensions.Logging;

namespace Core.Infra.DataAccess.Async;

internal class AsyncTaskSharedStore
{
    public const int FailedTryCount = 1;
    public const int FailedTryWaitMs = 0;

    private int _sequence = 0;
    internal readonly List<List<AsyncTaskItemStore>> _queue = new List<List<AsyncTaskItemStore>>();
    private readonly SemaphoreSlim _lockSemaphore = new SemaphoreSlim(1, 1);

    public async ValueTask<string[]> GetQueueIds()
    {
        await _lockSemaphore.WaitAsync();
        try
        {
            return _queue.SelectMany(i => i).Select(i => i?.Item?.QueueId ?? "").Distinct().ToArray();
        }
        finally
        {
            _lockSemaphore.Release();
        }
    }

    public async ValueTask<(AsyncTaskItemStore record, int level)?> Pop(string queueId)
    {
        await _lockSemaphore.WaitAsync();
        try
        {
            for (var lvl = 0; lvl < _queue.Count; ++lvl)
            {
                var queue = _queue[lvl];

                var idx = queue.FindIndex(0, i => i?.Item?.QueueId == queueId);
                if (idx >= 0)
                    return (queue[idx], lvl);
            }
            return null;
        }
        finally
        {
            _lockSemaphore.Release();
        }
    }

    public async ValueTask Update(IList<AsyncTaskItemStore>? scopeListToAdd, IList<(int lvl, AsyncTaskItemStore item)>? scopeListToDel)
    {
        await _lockSemaphore.WaitAsync();
        try
        {
            var now = DateTime.Now;
            if (scopeListToDel != null)
            {
                foreach (var itemToDel in scopeListToDel)
                {
                    _queue[itemToDel.lvl].Remove(itemToDel.item);
                    if (itemToDel.lvl > 0 && _queue[itemToDel.lvl].Count == 0 && itemToDel.lvl == _queue.Count - 1)
                    {
                        //clear empty levels
                        do
                        {
                            _queue.RemoveAt(_queue.Count - 1);
                        }
                        while (_queue.Count > 1 && _queue[^1].Count == 0);
                    }
                }
            }
            if (scopeListToAdd != null)
            {
                foreach (var itemToAdd in scopeListToAdd)
                {
                    itemToAdd.CreationDate = now;
                    itemToAdd.Id = ++_sequence;
                    var asyncTaskItem = itemToAdd.Item;
                    if (asyncTaskItem != null)
                    {
                        //New instance of AsyncTaskItem
                        itemToAdd.Item = new AsyncTaskItem()
                        {
                            QueueId = asyncTaskItem.QueueId,
                            Payload = asyncTaskItem.Payload,
                            PayloadType = asyncTaskItem.PayloadType,
                            HandlerType = asyncTaskItem.HandlerType,
                            WorkContextPayload = asyncTaskItem.WorkContextPayload,
                        };
                    }
                }
                if (scopeListToAdd.Count > 0)
                {
                    if (_queue.Count == 0)
                        _queue.Add(new List<AsyncTaskItemStore>());
                    _queue[0].AddRange(scopeListToAdd);
                }
            }
        }
        finally
        {
            _lockSemaphore.Release();
        }
    }

    public async ValueTask LevelUp(AsyncTaskItemStore record, int lvl, ILogger logger)
    {//level up item and lock it for some time
        await _lockSemaphore.WaitAsync();
        try
        {
            var now = DateTime.UtcNow;
            _queue[lvl].Remove(record);
            var levelUp = lvl + 1;
            if (levelUp <= FailedTryCount)
            {//Move to next level
                while (levelUp >= _queue.Count)
                    _queue.Add(new List<AsyncTaskItemStore>());
                _queue[levelUp].Add(record);
                record.LockUntil = now.AddMilliseconds(levelUp * FailedTryWaitMs);
            }
            else
            {//ignore task, remove from queue
                logger.LogError("Ignored task: {id} ({QueueId} - {PayloadType}), too many failed attempts: {FailedTryCount}", record.Id, record.Item?.QueueId, record.Item?.PayloadType, FailedTryCount);
                do
                {//Remove preceding empty levels
                    _queue.RemoveAt(_queue.Count - 1);
                }
                while (_queue.Count > 1 && _queue[^1].Count == 0);
            }
        }
        finally
        {
            _lockSemaphore.Release();
        }
    }
}