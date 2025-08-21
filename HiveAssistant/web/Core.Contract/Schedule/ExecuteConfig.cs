using Cronos;

namespace Core.Contract.Schedule;

public record ExecuteConfig()
{
    //Every day at 1 o'clock
    public const string DefaultCronExpression = "0 * * * *";

    private CronExpression? _cronExpression;

    /// <summary>
    /// Cron expression
    /// </summary>
    public string? Cron { get; set; }

    /// <summary>
    /// Period for job launched without special time, without persistent state
    /// </summary>
    public TimeSpan? Period { get; set; }

    /// <summary>
    /// Run on startup with
    /// </summary>
    public bool RunOnStart { get; set; } = false;

    /// <summary>
    /// Queue
    /// </summary>
    public string Queue { get; set; } = "Default";

    /// <summary>
    /// Max execution time - use for forecasting job end time
    /// If job don't end till this forecast it will be re-executed
    /// </summary>
    public TimeSpan? MaxExecuteTime { get; set; }

    public CronExpression? GetCronExpression()
    {
        try
        {

            if (Cron is not null)
                return _cronExpression ??= CronExpression.Parse(Cron, CronFormat.IncludeSeconds);
        }
        catch (Exception ex)
        {
            throw new ArgumentException(Cron, ex);
        }
        return null;
    }
}