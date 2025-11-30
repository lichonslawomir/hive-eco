using BeeHive.App.Hives.Repositories;
using BeeHive.Contract.Hives.Commands;
using Core.App.Handlers;

namespace BeeHive.App.Hives.Commands;

public sealed class UpdateHiveCommandHandler(IHiveRepository hiveRepository) : ICommandHandler<UpdateHiveCommand>
{
    public async Task HandleCommand(UpdateHiveCommand cmd, CancellationToken cancellationToken)
    {
        var hive = await hiveRepository.GetByIdAsync(cmd.Id, cancellationToken);
        if (hive is not null)
        {
            hive.Update(cmd.Data.Name,
                cmd.Data.UniqueKey,
                cmd.Data.ComPort,
                cmd.Data.GraphColor,
                cmd.Data.SerialBound,
                cmd.Data.AudioSensorSampleRate,
                cmd.Data.AudioSensorChannels,
                cmd.Data.AudioSensorBitsPerSample);
        }
    }
}