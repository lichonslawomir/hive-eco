using Core.Contract.Schedule;
using Microsoft.Extensions.Options;

namespace Core.Infra.Schedule;

public class JobCollection
{
    private readonly IOptions<Dictionary<string, ExecuteConfig>>? _configs;

    internal JobCollection(JobRecord[] jobs, IOptions<Dictionary<string, ExecuteConfig>>? configs)
    {
        Jobs = jobs;
        TimeZone = TimeZoneInfo.Utc;
        _configs = configs;
        Culture = "pl";
    }

    internal JobRecord[] Jobs { get; }

    public TimeZoneInfo TimeZone { get; set; }

    public string Culture { get; set; }

    internal ExecuteConfig GetJobConfig(JobRecord job)
    {
        if (_configs?.Value is null)
            return job.DefaultConfig;
        if (!_configs.Value.ContainsKey(job.Id))
            return job.DefaultConfig;
        return _configs.Value[job.Id];
    }
}