namespace Core.Infra.Schedule.InMemory;

internal class JobStateSharedStore
{
    private readonly SemaphoreSlim _lockSemaphore = new SemaphoreSlim(1, 1);
    private readonly IDictionary<string, JobState> _states = new Dictionary<string, JobState>();

    internal IDictionary<string, JobState> States { get { return _states; } }

    internal async Task<JobState> GetJobState(string jobId, Action<JobState>? inLockAction)
    {
        await _lockSemaphore.WaitAsync();
        try
        {
            if (!_states.TryGetValue(jobId, out JobState? state))
            {
                _states[jobId] = state = new JobState();
            }
            inLockAction?.Invoke(state);
            return state;
        }
        finally
        {
            _lockSemaphore.Release();
        }
    }
}

internal class InMemoryJobStateRepository : IJobStateRepository
{
    private readonly JobStateSharedStore _jobStateSharedStore;

    public InMemoryJobStateRepository(JobStateSharedStore jobStateSharedStore)
    {
        _jobStateSharedStore = jobStateSharedStore;
    }

    public async Task<bool> Book(string jobId, DateTimeOffset? prevBookDated, DateTimeOffset bookedUntil)
    {
        var booked = false;
        await _jobStateSharedStore.GetJobState(jobId, (s) =>
        {
            if (s.BookedUntilDate == prevBookDated)
            {
                s.BookedUntilDate = bookedUntil;
                booked = true;
            }
        });
        return booked;
    }

    public async Task<(DateTimeOffset? lastExecuted, DateTimeOffset? bookedUntil)> GetState(string jobId)
    {
        var s = await _jobStateSharedStore.GetJobState(jobId, null);
        return (s.LastExecuteDate, s.BookedUntilDate);
    }

    public async Task SetLastExecuteDate(string jobId, DateTimeOffset date)
    {
        await _jobStateSharedStore.GetJobState(jobId, (s) =>
        {
            s.LastExecuteDate = date;
            s.BookedUntilDate = null;
        });
    }
}