using BeeHive.App.Sensors;
using BeeHive.Domain.Data;

namespace Hive.Gateway.Service.Services;

public interface ISensorBuffor
{
    ValueTask AddAudio(string? hiveId, string comPort, byte[] data, CancellationToken stopToken);

    ValueTask AddData(string? hiveId, string comPort, TimeSeriesKind seriesKind, float data, CancellationToken stopToken);

    ValueTask<AudioData[]> ListAudio(CancellationToken stopToken);

    ValueTask<SensorData[]> ListData(CancellationToken stopToken);
}

internal class SensorBuffor : ISensorBuffor
{
    private readonly SemaphoreSlim _lockAudio = new(1, 1);
    private readonly Queue<AudioData> _audio = new();
    private readonly SemaphoreSlim _lockData = new(1, 1);
    private readonly Queue<SensorData> _data = new();

    public async ValueTask AddAudio(string? hiveId, string comPort, byte[] data, CancellationToken stopToken)
    {
        var timestamp = DateTimeOffset.Now;
        if (!await WaitLock(_lockAudio, stopToken))
            return;
        try
        {
            _audio.Enqueue(new AudioData()
            {
                ComPort = comPort,
                HiveId = hiveId,
                SeriesKind = TimeSeriesKind.BuzzFrequency,
                Data = data,
                Timestamp = timestamp
            });
        }
        finally
        {
            _lockAudio.Release();
        }
    }

    public async ValueTask AddData(string? hiveId, string comPort, TimeSeriesKind seriesKind, float data, CancellationToken stopToken)
    {
        if (data == float.NaN)
            return;

        var timestamp = DateTimeOffset.Now;
        if (!await WaitLock(_lockData, stopToken))
            return;
        try
        {
            _data.Enqueue(new SensorData()
            {
                ComPort = comPort,
                HiveId = hiveId,
                SeriesKind = seriesKind,
                Data = data,
                Timestamp = timestamp
            });
        }
        finally
        {
            _lockData.Release();
        }
    }

    public async ValueTask<AudioData[]> ListAudio(CancellationToken stopToken)
    {
        if (!await WaitLock(_lockAudio, stopToken))
            return Array.Empty<AudioData>();
        AudioData[] aa;
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

    public async ValueTask<SensorData[]> ListData(CancellationToken stopToken)
    {
        if (!await WaitLock(_lockData, stopToken))
            return Array.Empty<SensorData>();
        SensorData[] aa;
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