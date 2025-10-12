using BeeHive.App.Hives.Repositories;
using BeeHive.Contract.Hives.Commands;
using Core.App.Handlers;

namespace BeeHive.App.Hives.Commands;

public sealed class DeleteHiveMediaCommandHandler(IHiveMediaRepository hiveMediaRepository) : ICommandHandler<DeleteHiveMediaCommand>
{
    public async Task HandleCommand(DeleteHiveMediaCommand cmd, CancellationToken cancellationToken)
    {
        var media = await hiveMediaRepository.GetByIdAsync(cmd.Id, cancellationToken);

        media?.Delete();
    }
}