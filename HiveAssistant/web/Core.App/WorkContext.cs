using System.Globalization;
using System.Text.Json;

namespace Core.App;

public interface IWorkContext
{
    DateTime StartExecutionTime { get; }

    string Culture { get; }

    ExecutionType ExecutionType();

    void SetExecutionType(ExecutionType executionType);

    DateTime Now();

    string TimeZone();

    TUserId? GetUserId<TUserId>();

    string Serialize();

    void Derialize(string snapshot);
}

public abstract class WorkContext<TUserContext> : IWorkContext
{
    private DateTime? _startExecutionTime;
    private string? _culture;
    private ExecutionType _executionType;

    public TUserContext? User { get; set; }

    public DateTime StartExecutionTime
    {
        get => _startExecutionTime ??= Now();
        private set => _startExecutionTime = value;
    }

    public string Culture
    {
        get => _culture ??= CultureInfo.CurrentCulture.Name;
        private set => _culture = value;
    }

    public ExecutionType ExecutionType() => _executionType;

    public void SetExecutionType(ExecutionType executionType) => _executionType = executionType;

    public DateTime Now() => DateTime.UtcNow;

    public string TimeZone() => TimeZoneInfo.Local.Id;

    public abstract TUserId? GetUserId<TUserId>();

    public string Serialize()
    {
        return JsonSerializer.Serialize(this);
    }

    public void Derialize(string snapshot)
    {
        var snapshotObj = JsonSerializer.Deserialize(snapshot, this.GetType()) as WorkContext<TUserContext>;
        User = snapshotObj!.User;
        StartExecutionTime = snapshotObj!.StartExecutionTime;
        Culture = snapshotObj!.Culture;
    }
}