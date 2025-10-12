namespace Core.Infra.Schedule;

public interface IJobStateRepository
{
    Task<(DateTimeOffset? lastExecuted, DateTimeOffset? bookedUntil)> GetState(string jobId);

    Task<bool> Book(string jobId, DateTimeOffset? prevBookDated, DateTimeOffset bookedUntil);

    Task SetLastExecuteDate(string jobId, DateTimeOffset date);
}