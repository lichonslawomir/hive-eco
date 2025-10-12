using Core.Infra.Schedule.Extensioms.DependencyInjection;
using Hive.Gateway.Service.SerialPortSensores;
using Hive.Gateway.Service.Services;

namespace Hive.Gateway.Service.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGatewayServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddHostedService<ComBackgroundService>()
            .Configure<ComPortsOptions>(configuration.GetSection("ComPorts"))
            .RunJob<SensoreJob>(SensoreJob.DefaultExecuteConfig)

            .AddSingleton<ISensorBuffor, SensorBuffor>()
            .AddScoped<IHiveService, HiveService>();
    }
}