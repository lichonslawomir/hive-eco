using Core.App;
using Core.App.DataAccess;
using Core.App.Extensions;
using Core.App.Handlers.Async;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text.Json;

namespace Core.Infra.AsyncProcessors;

internal struct RunnerConfig
{
    //Max idle time - IdleMarkerInitState * IdleMarkerWait ms
    public int IdleMarkerInitState { get; set; }

    public int IdleMarkerWaitMs { get; set; }
}

internal class AsyncHandlerProcessorRunner
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IAsyncHandlerProcessorService _service;

    private readonly SemaphoreSlim _signalSemaphore = new(0);

    private readonly RunnerConfig _config;
    private int _idleMarker;

    public AsyncHandlerProcessorRunner(string queueId, IServiceProvider serviceProvider, IAsyncHandlerProcessorService service, RunnerConfig config)
    {
        QueueId = queueId;
        _serviceProvider = serviceProvider;
        _service = service;
        _config = config;
        _idleMarker = _config.IdleMarkerInitState;
        MainTask = Task.CompletedTask;
    }

    public void Signal()
    {
        Logger.LogInformation("Signal: {queueId}", QueueId);
        _idleMarker = _config.IdleMarkerInitState;
        _signalSemaphore.Release();
    }

    public bool IsIdle => _idleMarker <= 0;
    public string QueueId { get; }
    public Task MainTask { get; private set; }

    private ILogger<AsyncHandlerProcessorRunner> Logger =>
        _serviceProvider.GetRequiredService<ILogger<AsyncHandlerProcessorRunner>>();

    public void Run(CancellationToken cancellationToken)
    {
        MainTask = Task.Run(() => MainLoop(cancellationToken), cancellationToken);
    }

    private async Task MainLoop(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested && !IsIdle)
        {
            var waitMsTime = await ProcessQueueLoop(cancellationToken);

            if (!waitMsTime.HasValue)
                --_idleMarker;
            try
            {
                await _signalSemaphore.WaitAsync(waitMsTime ?? _config.IdleMarkerWaitMs, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                //ignore
            }
        }
        Logger.LogInformation("End MainLoop: {queueId}", QueueId);
        await _service.RemoveIdle(QueueId);
    }

    private async Task<int?> ProcessQueueLoop(CancellationToken cancellationToken)
    {
        Logger.LogInformation("ProcessQueueLoop: {queueId}", QueueId);
        var repositoryHasItems = true;
        int? waitMsTime = null;
        do
        {
            await using var scopeForAsyncTaskRepository = _serviceProvider.CreateAsyncScope();
            try
            {
                var asyncTaskRepository = scopeForAsyncTaskRepository.ServiceProvider.GetRequiredService<IAsyncTaskRepository>();
                (var queueItem, waitMsTime) = await asyncTaskRepository.Pop(QueueId);
                try
                {
                    if (queueItem != null)
                    {
                        await ProcessQueueItem(queueItem, asyncTaskRepository, cancellationToken);
                    }
                    else
                    {
                        repositoryHasItems = false;
                    }
                }
                catch (Exception outOfHandleException)
                {
                    Logger.LogError(outOfHandleException, "Out of handle exception: {queueId}", QueueId);
                    if (queueItem != null)
                        await asyncTaskRepository.SetHandleException(queueItem, outOfHandleException, true);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Queue exception: {queueId}", QueueId);
            }
        }
        while (!cancellationToken.IsCancellationRequested && repositoryHasItems);

        return waitMsTime;
    }

    private async Task ProcessQueueItem(AsyncTaskItem queueItem,
        IAsyncTaskRepository asyncTaskRepository,
        CancellationToken cancellationToken)
    {
        await using var scopeForUnitOfWork = _serviceProvider.CreateAsyncScope();

        var workContext = scopeForUnitOfWork.ServiceProvider.GetRequiredService<IWorkContext>();
        workContext.SetExecutionType(ExecutionType.BackgroundTask);

        var payloadType = !string.IsNullOrEmpty(queueItem.PayloadType) ? Type.GetType(queueItem.PayloadType.GetSlimName()) : null;
        if (payloadType is null)
            throw new NotSupportedException($"Payload type: {queueItem.PayloadType} not found");

        if (!string.IsNullOrEmpty(queueItem.WorkContextPayload))
        {
            workContext.Derialize(queueItem.WorkContextPayload);
            CultureInfo.CurrentCulture = new CultureInfo(workContext.Culture);
            CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture;
        }

        IInvoker? invoker = null;
        if (payloadType.IsCommandType())
        {
            var invokerType = typeof(CommandHandlerInvoker<>).MakeGenericType(payloadType);
            invoker = Activator.CreateInstance(invokerType, JsonSerializer.Deserialize(queueItem.Payload, payloadType)) as IInvoker;
        }
        if (payloadType.IsDomainEvent())
        {
            var invokerType = typeof(DomainEventHandlerInvoker<>).MakeGenericType(payloadType);
            invoker = Activator.CreateInstance(invokerType, JsonSerializer.Deserialize(queueItem.Payload, payloadType)) as IInvoker;
        }
        if (invoker is null)
            throw new NotSupportedException($"Invoker not initilized for types: {queueItem.PayloadType} - {queueItem.HandlerType}");

        try
        {
            await invoker.RunHandler(queueItem.HandlerType, scopeForUnitOfWork.ServiceProvider, cancellationToken);

            await scopeForUnitOfWork.ServiceProvider.GetRequiredService<IUnitOfWork>().CommitAsync(cancellationToken);
            await asyncTaskRepository.SaveHandleItems();
        }
        catch (Exception handleEx)
        {
            Logger.LogError(handleEx, "Handle exception: {queueId} - {commandType} - {handlerType}", QueueId, queueItem.PayloadType, queueItem.HandlerType);
            await asyncTaskRepository.SetHandleException(queueItem, handleEx, false);
        }
    }
}