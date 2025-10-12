using Core.Contract;

namespace Core.App.Handlers;

public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    int Order { get => 0; }

    Task HandleCommand(TCommand cmd, CancellationToken cancellationToken);
}