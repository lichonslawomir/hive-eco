namespace BeeHive.Contract.Hives.Models;

public class HiveUpdateModel
{
    public required string UniqueKey { get; set; }
    public required string Name { get; set; }

    public required string ComPort { get; set; }

    public required int GraphColor { get; set; }

    public required int AudioSensorSampleRate { get; set; }
    public required int AudioSensorChannels { get; set; }
    public required int AudioSensorBitsPerSample { get; set; }
}