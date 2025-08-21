using BeeHive.Domain.BeeGardens;
using BeeHive.Domain.Hives.Audio;
using BeeHive.Domain.Hives.Events;
using Core.Domain.Aggregates;

namespace BeeHive.Domain.Hives;

public class Hive : AuditableAggregateRoot<int, string>
{
    public string Name { get; protected set; }
    public string UniqueKey { get; protected set; }

    public BeeGarden BeeGarden { get; protected set; } = null!;
    public int BeeGardenId { get; protected set; }

    public int AudioSensorSampleRate { get; protected set; } = 16000;
    public int AudioSensorChannels { get; protected set; } = 1;
    public int AudioSensorBitsPerSample { get; protected set; } = 16;

    protected Hive(string name, string uniqueKey)
    {
        Name = name;
        UniqueKey = uniqueKey;
    }

    public static Hive Create(string name, string uniqueKey, BeeGarden beeGarden)
    {
        var hive = new Hive(name, uniqueKey)
        {
            BeeGarden = beeGarden
        };

        hive.PublishEvent(new NewHiveEvent(hive));

        return hive;
    }

    public AudioFile CreateAudioFile(DateTime timestamp, string fileName)
    {
        var audioFile = new AudioFile(this, timestamp, fileName);

        PublishEvent(new AddAudioDataEvent(this, fileName, false));

        return audioFile;
    }

    public void UpdateAudioFile(AudioFile audioFile, bool complete,
        float durationSec, float frequency, float amplitudePeak, float amplitudeRms, float amplitudeMav)
    {
        audioFile.Complete = complete;

        audioFile.DurationSec = durationSec;
        audioFile.Frequency = frequency;
        audioFile.AmplitudePeak = amplitudePeak;
        audioFile.AmplitudeRms = amplitudeRms;
        audioFile.AmplitudeMav = amplitudeMav;

        audioFile.SampleRate = this.AudioSensorSampleRate;
        audioFile.Channels = this.AudioSensorChannels;
        audioFile.BitsPerSample = this.AudioSensorBitsPerSample;

        PublishEvent(new AddAudioDataEvent(this, audioFile.FileName, complete));
    }
}