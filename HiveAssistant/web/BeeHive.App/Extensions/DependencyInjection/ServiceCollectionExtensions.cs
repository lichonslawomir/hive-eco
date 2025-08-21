using BeeHive.App.Aggragete.Services;
using BeeHive.App.Sensors;
using Microsoft.Extensions.DependencyInjection;

namespace BeeHive.App.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        return services
            .AddScoped<IAggrageteService, AggrageteService>()
            .AddScoped<ISensorService, SensorService>()
            .AddScoped<IAudioService, AudioService>();
    }
}