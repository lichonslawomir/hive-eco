namespace BeeHive.Contract.Hives.Models;

public struct HiveDto
{
    public required int Id { get; init; }
    public required string UniqueKey { get; init; }
    public required string Name { get; init; }
    public required string? ComPort { get; init; }
    public required string? LastComPortUsed { get; init; }

    public required int AudioSensorSampleRate { get; init; }
    public required int AudioSensorChannels { get; init; }
    public required int AudioSensorBitsPerSample { get; init; }
}