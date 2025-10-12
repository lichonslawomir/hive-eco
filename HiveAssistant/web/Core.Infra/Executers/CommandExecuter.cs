using Core.App;
using Core.App.DataAccess;
using Core.Contract;
using Core.Contract.Executers;

namespace Core.Infra.Executers;

public class CommandExecuter(ICommandBus commandBus, IUnitOfWork unitOfWork) : ICommandExecuter
{
    public async Task<ICommitResult> ExecuteCommand<TCommand>(TCommand cmd, CancellationToken cancellationToken) where TCommand : ICommand
    {
        await commandBus.ExecuteCommand<TCommand>(cmd, cancellationToken);

        return await unitOfWork.CommitAsync(cancellationToken);
    }
}