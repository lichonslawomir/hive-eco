using Core.Contract.Schedule;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Core.Infra.Schedule;

internal class ScheduleBackgroundService(IServiceProvider serviceProvider, JobCollection jobCollection, IScheduleDateTimeProvider scheduleDateTimeProvider, ILogger<ScheduleBackgroundService> logger) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation($"ExecuteAsync: {nameof(ScheduleBackgroundService)}");

        var jobLoopTasks = jobCollection.Jobs
            .GroupBy(x => jobCollection.GetJobConfig(x).Queue)
            .Select(k => Task.Factory.StartNew(
                () => RunJobs(k.Key, k.ToArray(), stoppingToken),
                stoppingToken)
            );
        return Task.WhenAll(jobLoopTasks);
    }

    private async Task RunJobs(string key, JobRecord[] jobs, CancellationToken stoppingToken)
    {
        var onStartJobs = jobs.Where(x => jobCollection.GetJobConfig(x).RunOnStart).ToArray();
        if (onStartJobs.Any())
        {
            await RunOnStartJobs(jobs, stoppingToken);
        }

        var periodJobs = jobs.Where(x => jobCollection.GetJobConfig(x).Period.HasValue).ToArray();
        var cronJobs = jobs.Where(x => !string.IsNullOrEmpty(jobCollection.GetJobConfig(x).Cron)).ToArray();

        Task? periodJobTask = null;
        if (!stoppingToken.IsCancellationRequested && periodJobs.Any())
        {
            periodJobTask = RunPeriodJobs(periodJobs, stoppingToken);
        }

        Task? cronJobTask = null;
        if (!stoppingToken.IsCancellationRequested && cronJobs.Any())
        {
            cronJobTask = RunCronJobs(cronJobs, stoppingToken);
        }

        if (cronJobTask is not null && periodJobTask is not null)
            await Task.WhenAll(cronJobTask, periodJobTask);
        else if (cronJobTask is not null)
            await cronJobTask;
        else if (periodJobTask is not null)
            await periodJobTask;
    }

    public async Task RunJob(IServiceProvider scopeProvider, Type jobType, CancellationToken stoppingToken)
    {
        CultureInfo.CurrentCulture = new CultureInfo(jobCollection.Culture);
        CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture;

        var obj = ActivatorUtilities.CreateInstance(scopeProvider, jobType);
        if (obj is IJob job)
        {
            await job.Execute(stoppingToken);
        }

        var disp = obj as IDisposable;
        disp?.Dispose();
    }

    public async Task RunOnStartJobs(JobRecord[] jobs, CancellationToken stoppingToken)
    {
        await scheduleDateTimeProvider.DelayOnStart(stoppingToken);

        foreach (var job in jobs)
        {
            await using var scope = serviceProvider.CreateAsyncScope();
            {
                try
                {
                    await RunJob(scope.ServiceProvider, job.JobType, stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Initial job exception: {job.Id}");
                }
            }
        }
    }

    public async Task RunPeriodJobs(JobRecord[] jobs, CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var job in jobs)
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                {
                    try
                    {
                        var config = jobCollection.GetJobConfig(job);
                        if (config.Period.HasValue)
                        {
                            await Task.Delay(config.Period.Value);
                        }
                        else
                            break;

                        await RunJob(scope.ServiceProvider, job.JobType, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"Initial job exception: {job.Id}");

                        await Task.Delay(30000);
                    }
                }
            }
        }
    }

    private async Task<List<(JobRecord job, DateTimeOffset nextOccurrence, DateTimeOffset? bookedUntil)>> CurrentState(JobRecord[] jobs, DateTimeOffset now)
    {
        var nexts = new List<(JobRecord job, DateTimeOffset nextOccurrence, DateTimeOffset? bookedUntil)>();

        AsyncServiceScope? scope = null;
        IJobStateRepository? jobStateRepository = null;
        try
        {
            foreach (var job in jobs)
            {
                var config = jobCollection.GetJobConfig(job);
                var cron = config.GetCronExpression();
                if (cron is null)
                    continue;

                if (jobStateRepository is null)
                {
                    if (scope is null)
                        scope = serviceProvider.CreateAsyncScope();
                    jobStateRepository = scope?.ServiceProvider.GetRequiredService<IJobStateRepository>();
                }
                if (jobStateRepository is null)
                    throw new NullReferenceException(nameof(jobStateRepository));

                try
                {
                    var (lastExecuted, bookedUntil) = await jobStateRepository.GetState(job.Id);
                    var nextOccurrence = bookedUntil ?? cron.GetNextOccurrence(lastExecuted ?? now, jobCollection.TimeZone);
                    if (nextOccurrence.HasValue)
                        nexts.Add((job, nextOccurrence.Value, bookedUntil));
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Schedule calculation {job.Id}");
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Schedule calculation");
        }
        finally
        {
            if (scope is IAsyncDisposable asyncDisp)
            {
                await asyncDisp.DisposeAsync();
            }
            else if (scope is IDisposable disp)
            {
                disp.Dispose();
            }
        }
        return nexts;
    }

    private async Task RunCronJobs(JobRecord[] jobs, CancellationToken stoppingToken)
    {
        var now = scheduleDateTimeProvider.UtcNow;
        var nexts = await CurrentState(jobs, now);
        DateTimeOffset nextOccurrence;
        if (nexts.Any())
            nextOccurrence = nexts.Min(j => j.nextOccurrence);
        else
            nextOccurrence = scheduleDateTimeProvider.UtcNow.AddMinutes(15);

        while (await Wait(nextOccurrence - now, stoppingToken))
        {
            now = scheduleDateTimeProvider.UtcNow;
            await using var repoScope = serviceProvider.CreateAsyncScope();
            await using var workingScope = serviceProvider.CreateAsyncScope();
            try
            {
                var jobStateRepository = repoScope.ServiceProvider.GetRequiredService<IJobStateRepository>();

                foreach (var next in nexts.OrderBy(j => j.nextOccurrence))
                {
                    now = scheduleDateTimeProvider.UtcNow;
                    if (now < next.nextOccurrence)
                    {
                        break;
                    }

                    var prevBookDated = next.bookedUntil;
                    var bookedUntil = ForecastEndTime(now, next.job);
                    await scheduleDateTimeProvider.DelayOnJobBook(stoppingToken);
                    if (await jobStateRepository.Book(next.job.Id, prevBookDated, bookedUntil))
                    {
                        logger.LogInformation("Run Job {JobId}", next.job.Id);

                        try
                        {
                            await RunJob(workingScope.ServiceProvider, next.job.JobType, stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Job {jobId} execution", next.job.Id);
                        }
                        await jobStateRepository.SetLastExecuteDate(next.job.Id, now = scheduleDateTimeProvider.UtcNow);
                    }
                }
                now = scheduleDateTimeProvider.UtcNow;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Job execution");
            }

            nexts = await CurrentState(jobs, now);
            if (nexts.Any())
                nextOccurrence = nexts.Min(j => j.nextOccurrence);
            else
                nextOccurrence = scheduleDateTimeProvider.UtcNow.AddMinutes(15);
        }
    }

    private async Task<bool> Wait(TimeSpan waitTime, CancellationToken stoppingToken)
    {
        try
        {
            if (stoppingToken.IsCancellationRequested)
                return false;
            if (waitTime >= TimeSpan.Zero)
                await scheduleDateTimeProvider.Wait(waitTime, stoppingToken);
            return true;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
    }

    private DateTimeOffset ForecastEndTime(DateTimeOffset now, JobRecord job)
    {
        var config = jobCollection.GetJobConfig(job);
        if (config.MaxExecuteTime.HasValue)
            return now.Add(config.MaxExecuteTime.Value);
        //If no MaxExecuteTime job can last till next cron occurrence
        return config.GetCronExpression()?.GetNextOccurrence(now, jobCollection.TimeZone) ?? now.AddDays(1);
    }
}