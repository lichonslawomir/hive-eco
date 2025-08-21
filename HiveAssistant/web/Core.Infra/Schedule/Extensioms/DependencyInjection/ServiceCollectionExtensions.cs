using Core.Contract.Schedule;
using Core.Infra.Schedule.InMemory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Core.Infra.Schedule.Extensioms.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RunJob<TJob>(this IServiceCollection services, ExecuteConfig defaultConfig) where TJob : IJob
    {
        services.AddTransient<JobRecord>(sp => new JobRecord(typeof(TJob).Name, typeof(TJob), defaultConfig));
        return services;
    }

    public static IServiceCollection AddJobSchedule<TJobStateRepository>(this IServiceCollection services, Action<JobCollection>? configure = null)
        where TJobStateRepository : class, IJobStateRepository
    {
        services.AddHostedService<ScheduleBackgroundService>();
        services.AddTransient<IScheduleDateTimeProvider, ScheduleDateTimeProvider>();
        services.AddScoped<IJobStateRepository, TJobStateRepository>();
        services.AddTransient<JobCollection>(sp =>
        {
            var jobRecords = sp.GetServices<JobRecord>();
            var jobCollection = new JobCollection(jobRecords.ToArray(),
                sp.GetService<IOptions<Dictionary<string, ExecuteConfig>>>());
            configure?.Invoke(jobCollection);
            return jobCollection;
        });
        return services;
    }

    public static IServiceCollection AddJobSchedule(this IServiceCollection services, Action<JobCollection>? configure = null)
    {
        return services.AddJobSchedule<InMemoryJobStateRepository>(configure)
            .AddSingleton<JobStateSharedStore>();
    }
}