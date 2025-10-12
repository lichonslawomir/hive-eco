using BeeHive.Domain.Aggregate;
using BeeHive.Domain.Aggregate.Extensions;
using BeeHive.Domain.Hives.Audio;
using Core.App;
using Microsoft.EntityFrameworkCore;

namespace BeeHive.App.Sensors;

public interface IAudioService
{
    Task SaveData(string holdingKey, string beeGardenKey,
        IEnumerable<(string? hiveId, byte[] data, DateTime timestamp)> values);
}

internal class AudioService : BaseDataService, IAudioService
{
    private readonly TimeZoneInfo _timeZoneInfo;
    private readonly IStorageManager _storageManager;

    public AudioService(IBeeHiveDbContext beeHiveDbContext,
        IWorkContext workContext,
        IStorageManager storageManager) : base(beeHiveDbContext, workContext)
    {
        _timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(_workContext.TimeZone());
        _storageManager = storageManager;
    }

    public async Task SaveData(string holdingKey, string beeGardenKey,
        IEnumerable<(string? hiveId, byte[] data, DateTime timestamp)> values)
    {
        var (beeGarden, newBeeGarden) = await GetBeeGarden(holdingKey, beeGardenKey);

        foreach (var kv in values.GroupBy(x => x.hiveId ?? string.Empty))
        {
            var hievKey = kv.Key;

            var (hive, newHive) = await GetHive(hievKey, beeGarden, newBeeGarden);

            AudioFile? audioFile = null;
            FileStream? fileStream = null;
            foreach (var d in kv.OrderBy(x => x.timestamp))
            {
                var tsOffset = _timeZoneInfo.ToDateTimeOffset(d.timestamp);
                if (audioFile is null ||
                    _timeZoneInfo.ToDateTimeOffset(audioFile.Timestamp).AdjustTo(AggregationPeriod.Min5) != tsOffset.AdjustTo(AggregationPeriod.Min5))
                {
                    if (fileStream is not null)
                    {
                        fileStream.Dispose();
                        fileStream = null;
                    }
                    if (audioFile is not null)
                    {
                        var audioFilePath = _storageManager.GetAudioFilePath(hievKey, audioFile.FileName);
                        byte[] data = File.ReadAllBytes(audioFilePath);
                        var (durationSec, frequency, amplitudePeak, amplitudeRms, amplitudeMav) = data.GetAdioStreamStats(audioFile.SampleRate, audioFile.Channels, audioFile.BitsPerSample);
                        hive.UpdateAudioFile(audioFile, false, durationSec, frequency, amplitudePeak, amplitudeRms, amplitudeMav);
                    }
                    var ts = tsOffset.AdjustTo(AggregationPeriod.Min5).UtcDateTime;
                    audioFile = newHive ? null : await _beeHiveDbContext.AudioFiles.FirstOrDefaultAsync(x => x.HiveId == hive.Id && x.Timestamp == ts);

                    if(audioFile is null)
                    {
                        audioFile = hive.CreateAudioFile(ts, CreateFileName(ts, hievKey, holdingKey, beeGardenKey));
                        _beeHiveDbContext.AudioFiles.Add(audioFile);
                    }
                }
                if (fileStream is null)
                {
                    fileStream = new FileStream(_storageManager.GetAudioFilePath(hievKey, audioFile.FileName), FileMode.OpenOrCreate);
                }
                await fileStream.WriteAsync(d.data, 0, d.data.Length);
            }

            if (fileStream is not null)
            {
                fileStream.Dispose();
                fileStream = null;
            }
            if (audioFile is not null)
            {
                var audioFilePath = _storageManager.GetAudioFilePath(hievKey, audioFile.FileName);
                byte[] data = File.ReadAllBytes(audioFilePath);
                var (durationSec, frequency, amplitudePeak, amplitudeRms, amplitudeMav) = data.GetAdioStreamStats(audioFile.SampleRate, audioFile.Channels, audioFile.BitsPerSample);
                hive.UpdateAudioFile(audioFile, true, durationSec, frequency, amplitudePeak, amplitudeRms, amplitudeMav);
            }

            var result = kv.SelectMany(x => x.data).GetAdioStreamStats(hive.AudioSensorSampleRate, hive.AudioSensorChannels, hive.AudioSensorBitsPerSample);
            var timeSeriesFrequency = await GetTimeSeries(Domain.Data.TimeSeriesKind.BuzzFrequency, hive, newHive);
            var timeSeriesAmplitudePeak = await GetTimeSeries(Domain.Data.TimeSeriesKind.BuzzAmplitudePeak, hive, newHive);
            var timeSeriesAmplitudeRms = await GetTimeSeries(Domain.Data.TimeSeriesKind.BuzzAmplitudeRms, hive, newHive);
            var timeSeriesAmplitudeMav = await GetTimeSeries(Domain.Data.TimeSeriesKind.BuzzAmplitudeMav, hive, newHive);

            timeSeriesFrequency.AddData([(kv.Min(x => x.timestamp), result.frequency)]);
            timeSeriesAmplitudePeak.AddData([(kv.Min(x => x.timestamp), result.amplitudePeak)]);
            timeSeriesAmplitudeRms.AddData([(kv.Min(x => x.timestamp), result.amplitudeRms)]);
            timeSeriesAmplitudeMav.AddData([(kv.Min(x => x.timestamp), result.amplitudeMav)]);
        }

        await _beeHiveDbContext.SaveChangesAsync();
    }

    public string CreateFileName(DateTime timestam, string holdingKey, string beeGardenKey, string hiveKey)
    {
        return $"{holdingKey}_{beeGardenKey}_{hiveKey}_{_timeZoneInfo.ToDateTimeOffset(timestam).ToString("yyyyMMdd_HHmm")}.pcm";
    }
}