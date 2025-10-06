using BeeHive.App.Hives.Repositories;
using BeeHive.Contract.Hives.Commands;
using Core.App.Handlers;

namespace BeeHive.App.Hives.Commands;

public sealed class UpdateHiveMediaCommandHandler(IHiveMediaRepository hiveMediaRepository) : ICommandHandler<UpdateHiveMediaCommand>
{
    public async Task HandleCommand(UpdateHiveMediaCommand cmd, CancellationToken cancellationToken)
    {
        var media = await hiveMediaRepository.GetByIdAsync(cmd.Id, cancellationToken);

        media?.Update(cmd.Data.Title);
    }
}