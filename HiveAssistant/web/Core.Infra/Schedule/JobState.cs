namespace Core.Infra.Schedule;

public class JobState
{
    public DateTimeOffset? LastExecuteDate { get; set; }
    public DateTimeOffset? BookedUntilDate { get; set; }
}