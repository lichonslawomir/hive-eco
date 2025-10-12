using Core.Contract;

namespace Core.App.Handlers;

public interface ICommandAsyncHandler<in TCommand> where TCommand : ICommand
{
    int Order { get; }

    ValueTask<string> AsyncTaskId(TCommand cmd);

    Task HandleCommand(TCommand cmd, CancellationToken cancellationToken);
}