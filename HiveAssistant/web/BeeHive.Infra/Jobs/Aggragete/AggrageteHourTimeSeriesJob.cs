using BeeHive.App.Aggragete.Services;
using BeeHive.Domain.Aggregate;
using Core.App;
using Core.App.DataAccess;
using Core.Contract.Schedule;

namespace BeeHive.Infra.Jobs.Aggragete;

public class AggrageteHourTimeSeriesJob(IAggrageteService aggrageteService,
    IUnitOfWork unitOfWork,
    IWorkContext workContext) : IJob
{
    public static ExecuteConfig DefaultExecuteConfig = new()
    {
        RunOnStart = true,
        Cron = "15 */5 * * * *",
        Queue = nameof(Aggragete),
    };

    public async Task Execute(CancellationToken stoppingToken)
    {
        var now = workContext.Now();
        await aggrageteService.UpdateAggragetes(now, workContext.TimeZone(), AggregationPeriod.Hour, stoppingToken);
        await unitOfWork.CommitAsync(stoppingToken);
    }
}