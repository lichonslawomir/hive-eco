namespace Core.Infra.Schedule;

public interface IScheduleDateTimeProvider
{
    /// <summary>
    /// Date and time
    /// </summary>
    DateTimeOffset UtcNow { get; }

    /// <summary>
    /// Delay on start
    /// </summary>
    /// <returns></returns>
    Task DelayOnStart(CancellationToken cancellationToken = default);

    /// <summary>
    /// Delay on job book
    /// </summary>
    /// <returns></returns>
    Task DelayOnJobBook(CancellationToken cancellationToken = default);

    /// <summary>
    /// Wait task
    /// </summary>
    /// <param name="waitTime"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task Wait(TimeSpan waitTime, CancellationToken cancellationToken = default);
}

internal class ScheduleDateTimeProvider : IScheduleDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;

    public Task DelayOnStart(CancellationToken cancellationToken = default)
    {
        //Random wait - from 1s to 5 min to avoid multi instance collisions in job executing
        return Task.Delay(TimeSpan.FromSeconds(Random.Shared.Next(1, 5 * 60)), cancellationToken);
    }

    public Task DelayOnJobBook(CancellationToken cancellationToken = default)
    {
        //Random wait - from 0s to 5sec min to avoid multi instance collisions in job executing
        return Task.Delay(TimeSpan.FromMilliseconds(Random.Shared.Next(0, 5 * 1000)), cancellationToken);
    }

    public Task Wait(TimeSpan waitTime, CancellationToken cancellationToken = default)
    {
        return Task.Delay(waitTime, cancellationToken);
    }
}