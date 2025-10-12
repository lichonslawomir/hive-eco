using BeeHive.App.Sensors;
using Core.App.DataAccess;
using Core.Contract.Schedule;
using Hive.Gateway.Service.Models;
using Hive.Gateway.Service.Services;
using Microsoft.Extensions.Options;

namespace Hive.Gateway.Service.SerialPortSensores;

public class SensoreJob(
    ISensorService sensorService,
    IAudioService audioService,
    ISensorBuffor sensorBuffor,
    IUnitOfWork unitOfWork,
    IOptions<BeeGardenConfig> beeGardenConfig) : IJob
{
    public static ExecuteConfig DefaultExecuteConfig = new()
    {
        Period = TimeSpan.FromSeconds(20),
        MaxExecuteTime = TimeSpan.FromMinutes(5)
    };

    public async Task Execute(CancellationToken cancellationToken)
    {
        var data = await sensorBuffor.ListData(cancellationToken);
        await sensorService.SaveData(beeGardenConfig.Value.HoldingKey, beeGardenConfig.Value.BeeGardenKey, data, cancellationToken);

        var audio = await sensorBuffor.ListAudio(cancellationToken);
        await audioService.SaveData(beeGardenConfig.Value.HoldingKey, beeGardenConfig.Value.BeeGardenKey, audio, cancellationToken);

        await unitOfWork.CommitAsync(cancellationToken);
    }
}