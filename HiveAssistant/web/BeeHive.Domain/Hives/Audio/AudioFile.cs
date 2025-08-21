namespace BeeHive.Domain.Hives.Audio;

public class AudioFile
{
    public Hive Hive { get; private set; } = null!;
    public int HiveId { get; private set; }

    public DateTime Timestamp { get; private set; }

    public string FileName { get; private set; } = null!;

    public int SampleRate { get; internal set; } = 16000;
    public int Channels { get; internal set; } = 1;
    public int BitsPerSample { get; internal set; } = 16;

    public bool Complete { get; internal set; }

    public float DurationSec { get; internal set; }
    public float Frequency { get; internal set; }
    public float AmplitudePeak { get; internal set; }
    public float AmplitudeRms { get; internal set; }
    public float AmplitudeMav { get; internal set; }

    public AudioFile(Hive hive, DateTime timestamp, string fileName)
    {
        Hive = hive;
        Timestamp = timestamp;
        FileName = fileName;

        SampleRate = hive.AudioSensorSampleRate;
        Channels = hive.AudioSensorChannels;
        BitsPerSample = hive.AudioSensorBitsPerSample;
    }

    protected AudioFile()
    {
    }
}