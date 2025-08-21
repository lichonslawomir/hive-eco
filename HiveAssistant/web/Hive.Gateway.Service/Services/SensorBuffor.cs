using BeeHive.Domain.Data;

namespace Hive.Gateway.Service.Services;

public interface ISensorBuffor
{
    ValueTask AddAudio(string? hiveId, byte[] data, CancellationToken stopToken);

    ValueTask AddData(string? hiveId, TimeSeriesKind seriesKind, float data, CancellationToken stopToken);

    ValueTask<(string? hiveId, byte[] data, DateTime timestamp)[]> ListAudio(CancellationToken stopToken);

    ValueTask<(string? hiveId, TimeSeriesKind seriesKind, float data, DateTime timestamp)[]> ListData(CancellationToken stopToken);
}

internal class SensorBuffor : ISensorBuffor
{
    private readonly SemaphoreSlim _lockAudio = new(1, 1);
    private readonly Queue<(string? hiveId, byte[] data, DateTime timestamp)> _audio = new();
    private readonly SemaphoreSlim _lockData = new(1, 1);
    private readonly Queue<(string? hiveId, TimeSeriesKind seriesKind, float data, DateTime timestamp)> _data = new();

    public async ValueTask AddAudio(string? hiveId, byte[] data, CancellationToken stopToken)
    {
        var timestamp = DateTime.UtcNow;
        if (!await WaitLock(_lockAudio, stopToken))
            return;
        try
        {
            _audio.Enqueue((hiveId, data, timestamp));
        }
        finally
        {
            _lockAudio.Release();
        }
    }

    public async ValueTask AddData(string? hiveId, TimeSeriesKind seriesKind, float data, CancellationToken stopToken)
    {
        if (data == float.NaN)
            return;

        var timestamp = DateTime.UtcNow;
        if (!await WaitLock(_lockData, stopToken))
            return;
        try
        {
            _data.Enqueue((hiveId, seriesKind, data, timestamp));
        }
        finally
        {
            _lockData.Release();
        }
    }

    public async ValueTask<(string? hiveId, byte[] data, DateTime timestamp)[]> ListAudio(CancellationToken stopToken)
    {
        if (!await WaitLock(_lockAudio, stopToken))
            return Array.Empty<(string? hiveId, byte[] data, DateTime timestamp)>();
        (string? hiveId, byte[] data, DateTime timestamp)[] aa;
        try
        {
            aa = _audio.ToArray();
            _audio.Clear();
        }
        finally
        {
            _lockAudio.Release();
        }
        return aa;
    }

    public async ValueTask<(string? hiveId, TimeSeriesKind seriesKind, float data, DateTime timestamp)[]> ListData(CancellationToken stopToken)
    {
        if (!await WaitLock(_lockData, stopToken))
            return Array.Empty<(string? hiveId, TimeSeriesKind seriesKind, float data, DateTime timestamp)>();
        (string? hiveId, TimeSeriesKind seriesKind, float data, DateTime timestamp)[] aa;
        try
        {
            aa = _data.ToArray();
            _data.Clear();
        }
        finally
        {
            _lockData.Release();
        }
        return aa;
    }

    private async ValueTask<bool> WaitLock(SemaphoreSlim ssemaphore, CancellationToken stopToken)
    {
        try
        {
            await ssemaphore.WaitAsync(stopToken);
            return true;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
    }
}