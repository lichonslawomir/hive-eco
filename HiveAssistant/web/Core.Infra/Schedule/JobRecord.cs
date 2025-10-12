using Core.Contract.Schedule;

namespace Core.Infra.Schedule;

internal class JobRecord
{
    public JobRecord(string id, Type jobType, ExecuteConfig defaultConfig)
    {
        Id = id;
        JobType = jobType;
        DefaultConfig = defaultConfig;
    }

    public string Id { get; private set; }
    public Type JobType { get; private set; }
    public ExecuteConfig DefaultConfig { get; private set; }
}