namespace Core.App;

public interface IWorkContext
{
    DateTime StartExecutionTime { get; }

    ExecutionContextType ExecutionContextType { get; }

    DateTime Now();

    string TimeZone();

    TUserId? GetUserId<TUserId>();
}

public abstract class WorkContext<TUserContext> : IWorkContext
{
    public TUserContext? User { get; set; }

    public ExecutionContextType ExecutionContextType { get; }

    public DateTime StartExecutionTime { get; set; }

    public DateTime Now() => DateTime.UtcNow;

    public string TimeZone() => TimeZoneInfo.Local.Id;

    public abstract TUserId? GetUserId<TUserId>();
}