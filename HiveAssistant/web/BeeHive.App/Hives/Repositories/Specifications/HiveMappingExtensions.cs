using System.Linq.Expressions;
using BeeHive.Contract.Hives.Models;
using BeeHive.Domain.Hives;

namespace BeeHive.App.Hives.Repositories.Specifications;

public static class HiveMappingExtensions
{
    public static HiveDto ToDto(this Hive e)
    {
        return new HiveDto()
        {
            Id = e.Id,
            Name = e.Name,
            UniqueKey = e.UniqueKey,
            ComPort = e.ComPort,
            LastComPortUsed = e.LastComPortUsed,
            GraphColor = e.GraphColor,
            SerialBound = e.SerialBound,
            AudioSensorBitsPerSample = e.AudioSensorBitsPerSample,
            AudioSensorChannels = e.AudioSensorChannels,
            AudioSensorSampleRate = e.AudioSensorSampleRate
        };
    }

    public static Expression<Func<Hive, HiveDto>> Map = e => new HiveDto()
    {
        Id = e.Id,
        Name = e.Name,
        UniqueKey = e.UniqueKey,
        ComPort = e.ComPort,
        LastComPortUsed = e.LastComPortUsed,
        GraphColor = e.GraphColor,
        SerialBound = e.SerialBound,
        AudioSensorBitsPerSample = e.AudioSensorBitsPerSample,
        AudioSensorChannels = e.AudioSensorChannels,
        AudioSensorSampleRate = e.AudioSensorSampleRate
    };

    public static HiveMediaDto ToDto(this HiveMedia e)
    {
        return new HiveMediaDto()
        {
            Id = e.Id,
            Url = e.Url,
            LocalPath = e.LocalPath,
            Title = e.Title,
            MediaType = e.MediaType
        };
    }

    public static Expression<Func<HiveMedia, HiveMediaDto>> MapMedia = e => new HiveMediaDto()
    {
        Id = e.Id,
        Url = e.Url,
        LocalPath = e.LocalPath,
        Title = e.Title,
        MediaType = e.MediaType
    };
}