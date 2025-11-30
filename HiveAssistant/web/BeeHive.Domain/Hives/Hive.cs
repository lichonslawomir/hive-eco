using System.Drawing;
using BeeHive.Domain.BeeGardens;
using BeeHive.Domain.Hives.Audio;
using BeeHive.Domain.Hives.Events;
using Core.Domain.Aggregates;

namespace BeeHive.Domain.Hives;

public sealed class Hive : AuditableAggregateRoot<int, string>, ISynchronizableEntity
{
    private readonly List<HiveMedia> _media = new();

    public string Name { get; private set; }
    public string UniqueKey { get; private set; }

    public BeeGarden BeeGarden { get; private set; } = null!;
    public int BeeGardenId { get; private set; }

    public string? ComPort { get; private set; }
    public string? LastComPortUsed { get; private set; }

    public int GraphColor { get; private set; } = Color.Blue.ToArgb();

    public int SerialBound { get; private set; } = 921600;
    public int AudioSensorSampleRate { get; private set; } = 16000;
    public int AudioSensorChannels { get; private set; } = 1;
    public int AudioSensorBitsPerSample { get; private set; } = 16;

    public DateTime CreatedOrUpdatedDate { get; private set; }

    public IReadOnlyCollection<HiveMedia> Media { get => _media; }

    private Hive(string name, string uniqueKey)
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

    public void UpdateComPort(string comPort)
    {
        if (string.IsNullOrEmpty(ComPort))
        {
            ComPort = comPort;
            LastComPortUsed = null;
        }
        else if (ComPort != comPort)
        {
            LastComPortUsed = comPort;
        }
        else
        {
            LastComPortUsed = null;
        }
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

    public HiveMedia CreateMedia(string? url,
        string? localPath,
        string title,
        MediaType type)
    {
        var media = new HiveMedia(this, url, localPath, title, type);

        _media.Add(media);

        return media;
    }

    public void Update(string name, string uniqueKey, string comPort, int graphColor, int serialBound, int audioSensorSampleRate, int audioSensorChannels, int audioSensorBitsPerSample)
    {
        Name = name;
        UniqueKey = uniqueKey;

        ComPort = comPort;
        GraphColor = graphColor;

        SerialBound = serialBound;
        AudioSensorSampleRate = audioSensorSampleRate;
        AudioSensorChannels = audioSensorChannels;
        AudioSensorBitsPerSample = audioSensorBitsPerSample;
    }

    public void Update(string name, string? comPort, int graphColor, int serialBound, int audioSensorSampleRate, int audioSensorChannels, int audioSensorBitsPerSample)
    {
        Name = name;
        ComPort = comPort;
        GraphColor = graphColor;

        SerialBound = serialBound;
        AudioSensorSampleRate = audioSensorSampleRate;
        AudioSensorChannels = audioSensorChannels;
        AudioSensorBitsPerSample = audioSensorBitsPerSample;
    }
}