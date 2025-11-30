using System.Linq.Expressions;
using BeeHive.Contract.BeeGardens.Models;
using BeeHive.Contract.Export;
using BeeHive.Contract.Hives.Models;
using BeeHive.Contract.Holdings.Models;
using BeeHive.Domain.Hives;

namespace BeeHive.App.Hives.Repositories.Specifications.Export;

public static class MapExportExtensions
{
    public static Expression<Func<Hive, HiveExportModel>> MapExport = e => new HiveExportModel()
    {
        Hive = new HiveDto()
        {
            Id = e.Id,
            Name = e.Name,
            UniqueKey = e.UniqueKey,
            ComPort = e.ComPort,
            GraphColor = e.GraphColor,
            LastComPortUsed = e.LastComPortUsed,
            SerialBound = e.SerialBound,
            AudioSensorSampleRate = e.AudioSensorSampleRate,
            AudioSensorChannels = e.AudioSensorChannels,
            AudioSensorBitsPerSample = e.AudioSensorBitsPerSample
        },
        BeeGarden = new BeeGardenDto()
        {
            Id = e.BeeGarden.Id,
            Name = e.BeeGarden.Name,
            UniqueKey = e.BeeGarden.UniqueKey,
            TimeZone = e.BeeGarden.TimeZone
        },
        Holding = new HoldingDto()
        {
            Id = e.BeeGarden.Holding.Id,
            UniqueKey = e.BeeGarden.Holding.UniqueKey,
            Name = e.BeeGarden.Holding.Name,
        },

        CreatedOrUpdatedDate = e.CreatedOrUpdatedDate
    };

    public static Expression<Func<HiveMedia, HiveMediaExportModel>> MapMediaExport = e => new HiveMediaExportModel()
    {
        HoldingUniqueKey = e.Hive.BeeGarden.Holding.UniqueKey,
        BeeGardenUniqueKey = e.Hive.BeeGarden.UniqueKey,
        HiveUniqueKey = e.Hive.UniqueKey,

        Id = e.Id,
        IsDeleted = e.IsDeleted,

        CreatedBy = e.CreatedBy,
        CreatedDate = e.CreatedDate,
        CreatedOrUpdatedDate = e.CreatedOrUpdatedDate,

        LocalPath = e.LocalPath,
        MediaType = e.MediaType,
        Title = e.Title,
        Url = e.Url
    };
}