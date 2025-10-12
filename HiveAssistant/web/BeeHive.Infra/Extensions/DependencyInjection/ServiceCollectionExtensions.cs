using BeeHive.App;
using BeeHive.App.Aggregate.Repositories;
using BeeHive.App.Data.Repositories;
using BeeHive.App.Hives.Repositories;
using BeeHive.Infra.DataAccess;
using BeeHive.Infra.DataAccess.DbContexts;
using BeeHive.Infra.DataAccess.Repositories;
using BeeHive.Infra.Jobs.Aggragete;
using BeeHive.Infra.Services;
using Core.App;
using Core.Infra.DataAccess;
using Core.Infra.Schedule.Extensioms.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BeeHive.Infra.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddScoped<IWorkContext, WorkContext>()
            .AddScoped<IDatabaseInitializer, DatabaseInitializer>()
            .AddScoped<IStorageManager, StorageManager>()
            .AddBeeHiveDbContext(configuration)
            .AddRepositories()
            .AddJobs();
    }

    public static IServiceCollection AddBeeHiveDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(nameof(BeeHiveDbContext));
        services.AddDbContext<BeeHiveDbContext>(options =>
            options.UseSqlite(connectionString)
        );
        services.AddScoped<IBeeHiveDbContext>(provider => provider.GetRequiredService<BeeHiveDbContext>());

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services
            .AddScoped<IHiveRepository, HiveRepository>()
            .AddScoped<ITimeSeriesDataRepository, TimeSeriesDataRepository>()
            .AddScoped<ITimeAggregateSeriesDataRepository, TimeAggregateSeriesDataRepository>();
        return services;
    }

    public static IServiceCollection AddJobs(this IServiceCollection services)
    {
        services
            .RunJob<AggrageteShortTimeSeriesJob>(AggrageteShortTimeSeriesJob.DefaultExecuteConfig)
            .RunJob<AggrageteHourTimeSeriesJob>(AggrageteHourTimeSeriesJob.DefaultExecuteConfig)
            .RunJob<AggrageteDayTimeSeriesJob>(AggrageteDayTimeSeriesJob.DefaultExecuteConfig);
        return services;
    }
}