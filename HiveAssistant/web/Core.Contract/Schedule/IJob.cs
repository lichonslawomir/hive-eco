namespace Core.Contract.Schedule;

public interface IJob
{
    Task Execute(CancellationToken stoppingToken);
}