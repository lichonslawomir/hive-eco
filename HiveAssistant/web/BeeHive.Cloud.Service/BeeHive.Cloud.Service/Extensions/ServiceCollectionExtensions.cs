using BeeHive.Cloud.Service.Services;
using BeeHive.Contract.Interfaces;
using BeeHive.Infra.Services;
using Core.App.Extensions;

namespace BeeHive.Cloud.Service.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCloudServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddScoped<IHiveService, HiveService>()
            .AddScoped<IHiveMediaService, HiveMediaService>()
            .AddSingleton<AppState>()
            .AddSingleton<IAppState>(sp => sp.GetRequiredService<AppState>())
            .AddHandlerAssembly<AppState>();
    }
}