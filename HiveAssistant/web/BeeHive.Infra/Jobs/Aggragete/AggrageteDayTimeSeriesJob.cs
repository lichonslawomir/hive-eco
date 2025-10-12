using BeeHive.App.Aggragete.Services;
using BeeHive.Domain.Aggregate;
using Core.App;
using Core.Contract.Schedule;

namespace BeeHive.Infra.Jobs.Aggragete;

public class AggrageteDayTimeSeriesJob(IAggrageteService aggrageteService, IWorkContext workContext) : IJob
{
    public static ExecuteConfig DefaultExecuteConfig = new()
    {
        RunOnStart = true,
        Cron = "30 0 1 * * ?",
        Queue = nameof(Aggragete),
    };

    public async Task Execute(CancellationToken stoppingToken)
    {
        var now = workContext.Now();
        await aggrageteService.UpdateAggragetes(now, workContext.TimeZone(), AggregationPeriod.Day, stoppingToken);
        await aggrageteService.UpdateAggragetes(now, workContext.TimeZone(), AggregationPeriod.Week, stoppingToken);
        await aggrageteService.UpdateAggragetes(now, workContext.TimeZone(), AggregationPeriod.Month, stoppingToken);
    }
}