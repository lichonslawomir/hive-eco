using Core.App.Extensions;
using Core.Domain.DomainEvents;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Text;

namespace Core.App.Handlers.Async;

public class AsyncTaskItem
{
    public required string QueueId { get; init; }

    public required string Payload { get; init; }
    public required string PayloadType { get; init; }

    public required string HandlerType { get; init; }

    public required string WorkContextPayload { get; init; }
}

public interface IAsyncTaskRepository
{
    public ValueTask<string[]> GetQueueIds();

    public ValueTask<(AsyncTaskItem? item, int? waitMsTime)> Pop(string queueId);

    public ValueTask SaveHandleItems();

    public ValueTask SetHandleError(AsyncTaskItem item, string? errorMessage);

    public ValueTask SetHandleException(AsyncTaskItem item, Exception ex, bool outOfHandler);

    public ValueTask Push(AsyncTaskItem item);

    public ValueTask SavePushedItems();

    public void ClearPushedItems();
}