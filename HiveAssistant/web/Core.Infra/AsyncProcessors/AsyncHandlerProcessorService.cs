using Core.App.Handlers.Async;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Core.Infra.AsyncProcessors;

internal interface IAsyncHandlerProcessorService
{
    Task RemoveIdle(string taskId);
}

internal class AsyncHandlerProcessorService : IAsyncHandlerProcessor, IAsyncHandlerProcessorService, IHostedService
{
    private static readonly RunnerConfig RunnerConfig = new()
    {
        //Max idle time - 5 sek = 5 * 1000ms
        IdleMarkerInitState = 5,
        IdleMarkerWaitMs = 1000
    };

    private readonly IServiceProvider _serviceProvider;

    private readonly List<AsyncHandlerProcessorRunner> _runners = new();
    private readonly SemaphoreSlim _lockSemaphore = new(1, 1);

    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public AsyncHandlerProcessorService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Signal(IEnumerable<string> asyncTaskIds)
    {
        if (!await WaitLock())
            return;
        try
        {
            foreach (var queueId in asyncTaskIds)
            {
                var runner = _runners.Find(r => r.QueueId == queueId);
                if (runner != null)
                    runner.Signal();
                else
                {
                    runner = new AsyncHandlerProcessorRunner(queueId, _serviceProvider, this, RunnerConfig);
                    _runners.Add(runner);
                    runner.Run(_cancellationTokenSource.Token);
                }
            }
        }
        finally
        {
            _lockSemaphore.Release();
        }
    }

    public async Task RemoveIdle(string queueId)
    {
        if (!await WaitLock())
            return;
        try
        {
            var runner = _runners.Find(r => r.QueueId == queueId);
            if (runner != null && runner.IsIdle)
                _runners.Remove(runner);
        }
        finally
        {
            _lockSemaphore.Release();
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        string[] queueIds;
        await using (var scope = _serviceProvider.CreateAsyncScope())
        {
            var queueRepository = scope.ServiceProvider.GetRequiredService<IAsyncTaskRepository>();
            queueIds = await queueRepository.GetQueueIds();
        }
        await Signal(queueIds);
    }

    private async Task<bool> WaitLock()
    {
        try
        {
            await _lockSemaphore.WaitAsync(_cancellationTokenSource.Token);
            return true;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            _cancellationTokenSource.Cancel();
        }
        finally
        {
            var waitAllTasks = Task.WhenAll(_runners.Select(r => r.MainTask));
            await Task.WhenAny(waitAllTasks, Task.Delay(Timeout.Infinite, cancellationToken));
        }
    }
}