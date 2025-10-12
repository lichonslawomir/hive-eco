using BeeHive.App;
using Core.Infra.Schedule.Extensioms.DependencyInjection;
using Hive.Gateway.Service.Export;
using Hive.Gateway.Service.SerialPortSensores;
using Hive.Gateway.Service.Services;
using Core.App.Extensions;

namespace Hive.Gateway.Service.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGatewayServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .RunJob<ExportJob>(ExportJob.DefaultExecuteConfig)
            .AddHttpClient<IExportService, ExportService>(client =>
            {
                var exportSecret = configuration["ExportSecret"] ?? throw new NotSupportedException("ExportSecret");
                var exportServiceUrl = configuration["ExportServiceUrl"] ?? throw new NotSupportedException("ExportServiceUrl");

                client.BaseAddress = new Uri(exportServiceUrl);
                client.DefaultRequestHeaders.Add("ExportSecret", exportSecret);
            });

        return services
            .AddHostedService<ComBackgroundService>()
            .Configure<ComPortsOptions>(configuration.GetSection("ComPorts"))
            .RunJob<SensoreJob>(SensoreJob.DefaultExecuteConfig)

            .AddSingleton<ISensorBuffor, SensorBuffor>()
            .AddSingleton<AppState>()
            .AddScoped<IScopeService, ScopeService>()
            .AddScoped<IHiveService, HiveService>()
            .AddScoped<IHiveMediaService, HiveMediaService>()
            .AddHandlerAssembly<AppState>();
    }
}