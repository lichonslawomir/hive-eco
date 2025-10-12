using Core.App.Extensions;
using Core.Domain.DomainEvents;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace Core.App.Handlers.Async;

internal class AsyncDomainEventHandlerWrapperr<TDomainEventAsyncHandler, TDomainEvent> : IDomainEventHandler<TDomainEvent>
    where TDomainEvent : IDomainEvent
    where TDomainEventAsyncHandler : IDomainEventAsyncHandler<TDomainEvent>
{
    private readonly TDomainEventAsyncHandler _handler;
    private readonly IAsyncTaskSignaler _taskSignaler;
    private readonly IAsyncTaskRepository _asyncTaskRepository;
    private readonly IWorkContext _workContext;

    public AsyncDomainEventHandlerWrapperr(
        IServiceProvider serviceProvider,
        IAsyncTaskSignaler taskSignaler,
        IAsyncTaskRepository asyncTaskRepository,
        IWorkContext workContext)
    {
        _handler = (TDomainEventAsyncHandler)ActivatorUtilities.CreateInstance(serviceProvider, typeof(TDomainEventAsyncHandler));
        _taskSignaler = taskSignaler;
        _asyncTaskRepository = asyncTaskRepository;
        _workContext = workContext;
    }

    public int Order => _handler.Order;

    public async ValueTask HandleEvent(TDomainEvent cmd, CancellationToken cancellationToken)
    {
        var asyncTaskId = await _handler.AsyncTaskId(cmd);

        await _asyncTaskRepository.Push(new AsyncTaskItem()
        {
            QueueId = asyncTaskId,
            Payload = JsonSerializer.Serialize<TDomainEvent>(cmd),
            PayloadType = typeof(TDomainEvent).GetSlimName(),
            HandlerType = typeof(TDomainEventAsyncHandler).GetSlimName(),
            WorkContextPayload = _workContext.Serialize()
        });

        _taskSignaler.SignalOnCommit(asyncTaskId);
    }
}