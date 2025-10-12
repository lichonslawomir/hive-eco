using Core.App;
using Core.App.DataAccess;
using Core.App.Handlers.Async;
using Core.Contract.Executers;
using Core.Infra.AsyncProcessors;
using Core.Infra.DataAccess;
using Core.Infra.DataAccess.Async;
using Core.Infra.DataAccess.DbContexts;
using Core.Infra.Executers;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infra.Extensions;

public class InfraOptions
{
    public bool UseSoftHandlerProcessing { get; set; } = false;

    public required Type WorkContextType { get; set; }
    public required Type UnitOfWorkType { get; set; }
    public Type? AsyncTaskRepositoryType { get; set; }
}

public static class InfraOptionsExtensions
{
    public static void UseAsyncTaskRepositoryType<TAsyncTaskRepository>(this InfraOptions opt)
    {
        opt.AsyncTaskRepositoryType = typeof(TAsyncTaskRepository);
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreInfraServices<TWorkContext, TDbContext, TUserId>(this IServiceCollection services,
        Action<InfraOptions>? optionsAction = null)
        where TWorkContext : class, IWorkContext
        where TDbContext : BaseDbContext<TDbContext, TUserId>
    {
        var opt = new InfraOptions()
        {
            WorkContextType = typeof(TWorkContext),
            UnitOfWorkType = typeof(UnitOfWork<TDbContext, TUserId>)
        };
        optionsAction?.Invoke(opt);

        return services.AddCoreInfraServices(opt);
    }

    public static IServiceCollection AddCoreInfraServices(this IServiceCollection services, InfraOptions options)
    {
        if (options.AsyncTaskRepositoryType is null)
        {
            options.AsyncTaskRepositoryType = typeof(InMemoryAsyncTaskRepository);
            services.AddSingleton<AsyncTaskSharedStore>();
        }

        if (options.UseSoftHandlerProcessing)
        {
            services.AddSingleton<ISoftAsyncHandlerProcessor, SoftAsyncHandlerProcessor>()
                .AddTransient<IAsyncHandlerProcessor>(s => s.GetRequiredService<ISoftAsyncHandlerProcessor>());
        }
        else
        {
            //Handler processing base on background IHostedServices
            services.AddSingleton<AsyncHandlerProcessorService>();
            services.AddHostedService<AsyncHandlerProcessorService>(s => s.GetRequiredService<AsyncHandlerProcessorService>());
            services.AddTransient<IAsyncHandlerProcessor>(s => s.GetRequiredService<AsyncHandlerProcessorService>());
        }

        services.AddTransient<ICommandExecuter, CommandExecuter>()
            .AddScoped(typeof(IWorkContext), options.WorkContextType)
            .AddScoped(typeof(IAsyncTaskRepository), options.AsyncTaskRepositoryType)
            .AddScoped(typeof(IUnitOfWork), options.UnitOfWorkType);

        return services;
    }
}