using Core.App.Handlers.Async;

namespace Core.Infra.AsyncProcessors;

public interface ISoftAsyncHandlerProcessor : IAsyncHandlerProcessor
{
    Task WaitUntilDone(CancellationToken cancellationToken);
}

/// <summary>
/// Procesing handlers without background service
/// </summary>
internal class SoftAsyncHandlerProcessor : ISoftAsyncHandlerProcessor, IAsyncHandlerProcessorService
{
    private static readonly RunnerConfig RunnerConfig = new RunnerConfig()
    {
        //Max idle time - 20ms = 2 * 10ms
        IdleMarkerInitState = 2,
        IdleMarkerWaitMs = 10
    };

    private readonly IServiceProvider _serviceProvider;

    private readonly List<AsyncHandlerProcessorRunner> _runners = new List<AsyncHandlerProcessorRunner>();
    private readonly SemaphoreSlim _lockSemaphore = new SemaphoreSlim(1, 1);

    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    public SoftAsyncHandlerProcessor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task WaitUntilDone(CancellationToken cancellationToken)
    {
        int runnersCount = 0;
        do
        {
            Task[] runners;
            if (!await WaitLock())
                return;
            try
            {
                runners = _runners.Select(r => r.MainTask).ToArray();
            }
            finally
            {
                _lockSemaphore.Release();
            }

            runnersCount = runners.Length;
            if (runnersCount > 0)
            {
                var waitTask = Task.WhenAll(runners);
                await Task.WhenAny(waitTask, Task.Delay(Timeout.Infinite, cancellationToken));
                if (cancellationToken.IsCancellationRequested)
                {
                    _cancellationTokenSource.Cancel();
                }
            }
        }
        while (runnersCount > 0);
    }

    public async Task Signal(IEnumerable<string> queueIds)
    {
        if (!await WaitLock())
            return;
        try
        {
            foreach (var queueId in queueIds)
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
}