using BeeHive.App.Sensors;
using Core.Contract.Schedule;
using Hive.Gateway.Service.Models;
using Hive.Gateway.Service.Services;
using Microsoft.Extensions.Options;

namespace Hive.Gateway.Service.SerialPortSensores;

public class SensoreJob(
    ISensorService sensorService,
    IAudioService audioService,
    ISensorBuffor sensorBuffor,
    IOptions<BeeGardenConfig> beeGardenConfig) : IJob
{
    public static ExecuteConfig DefaultExecuteConfig = new()
    {
        Period = TimeSpan.FromSeconds(20),
        MaxExecuteTime = TimeSpan.FromMinutes(5)
    };

    public async Task Execute(CancellationToken stoppingToken)
    {
        var data = await sensorBuffor.ListData(stoppingToken);
        await sensorService.SaveData(beeGardenConfig.Value.HoldingKey, beeGardenConfig.Value.BeeGardenKey, data);

        var audio = await sensorBuffor.ListAudio(stoppingToken);
        await audioService.SaveData(beeGardenConfig.Value.HoldingKey, beeGardenConfig.Value.BeeGardenKey, audio);
    }
}