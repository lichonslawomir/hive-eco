namespace Core.Contract.Executers;

public interface ICommandExecuter
{
    Task<ICommitResult> ExecuteCommand<TCommand>(TCommand cmd, CancellationToken cancellationToken)
            where TCommand : ICommand;
}