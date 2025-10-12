using BeeHive.App.Aggragete.Services;
using BeeHive.App.Sensors;
using Core.App.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace BeeHive.App.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        return services
            .AddCoreAppServices()
            .AddHandlerAssembly<IBeeHiveDbContext>()
            .AddScoped<IAggrageteService, AggrageteService>()
            .AddScoped<ISensorService, SensorService>()
            .AddScoped<IAudioService, AudioService>();
    }
}