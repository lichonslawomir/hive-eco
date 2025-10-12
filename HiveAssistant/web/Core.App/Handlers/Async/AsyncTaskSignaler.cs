using Core.App.DataAccess;

namespace Core.App.Handlers.Async;

internal interface IAsyncTaskSignaler
{
    void SignalOnCommit(string asyncTaskId);
}

internal class AsyncTaskSignaler(
    IUnitOfWork unitOfWork,
    IAsyncHandlerProcessor handlerProcessor,
    IAsyncTaskRepository asyncTaskRepository) : IAsyncTaskSignaler
{
    private IList<string>? _taskToSignal;

    public void SignalOnCommit(string asyncTaskId)
    {
        if (_taskToSignal is null)
        {
            _taskToSignal = new List<string>();
            unitOfWork.CommitEvent += UnitOfWork_Commit;
        }

        if (!_taskToSignal.Contains(asyncTaskId))
        {
            _taskToSignal.Add(asyncTaskId);
        }
    }

    private async ValueTask UnitOfWork_Commit(CancellationToken cancellationToken)
    {
        //Required for inmemory implementations
        await asyncTaskRepository.SavePushedItems();
        await handlerProcessor.Signal(_taskToSignal ?? Array.Empty<string>());
    }
}