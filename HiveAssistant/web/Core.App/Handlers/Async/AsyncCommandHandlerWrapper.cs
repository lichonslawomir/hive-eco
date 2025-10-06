using Core.App.Extensions;
using Core.Contract;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace Core.App.Handlers.Async;

internal class AsyncCommandHandlerWrapperr<TCommandAsyncHandler, TCommand> : ICommandHandler<TCommand>
    where TCommand : ICommand
    where TCommandAsyncHandler : ICommandAsyncHandler<TCommand>
{
    private readonly TCommandAsyncHandler _handler;
    private readonly IAsyncTaskSignaler _taskSignaler;
    private readonly IAsyncTaskRepository _asyncTaskRepository;
    private readonly IWorkContext _workContext;

    public AsyncCommandHandlerWrapperr(
        IServiceProvider serviceProvider,
        IAsyncTaskSignaler taskSignaler,
        IAsyncTaskRepository asyncTaskRepository,
        IWorkContext workContext)
    {
        _handler = (TCommandAsyncHandler)ActivatorUtilities.CreateInstance(serviceProvider, typeof(TCommandAsyncHandler));
        _taskSignaler = taskSignaler;
        _asyncTaskRepository = asyncTaskRepository;
        _workContext = workContext;
    }

    public int Order => _handler.Order;

    public async Task HandleCommand(TCommand cmd, CancellationToken cancellationToken)
    {
        var asyncTaskId = await _handler.AsyncTaskId(cmd);

        await _asyncTaskRepository.Push(new AsyncTaskItem()
        {
            QueueId = asyncTaskId,
            Payload = JsonSerializer.Serialize<TCommand>(cmd),
            PayloadType = typeof(TCommand).GetSlimName(),
            HandlerType = typeof(TCommandAsyncHandler).GetSlimName(),
            WorkContextPayload = _workContext.Serialize()
        });

        _taskSignaler.SignalOnCommit(asyncTaskId);
    }
}