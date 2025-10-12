using BeeHive.Contract.Export;
using Core.App;
using Core.App.Handlers;
using Microsoft.EntityFrameworkCore;

namespace BeeHive.App.Export;

public class ImportDataCommandHandler : BaseDataExportService, ICommandHandler<ImportDataCommand>
{
    public ImportDataCommandHandler(IBeeHiveDbContext beeHiveDbContext, IWorkContext workContext) : base(beeHiveDbContext, workContext)
    {
    }

    public async Task HandleCommand(ImportDataCommand cmd, CancellationToken cancellationToken)
    {
        if (cmd.Hives is not null)
        {
            foreach (var model in cmd.Hives)
            {
                var (holding, newHolding) = await GetHolding(model.Holding.UniqueKey, cancellationToken);
                if (!newHolding)
                {
                    holding.Update(model.Holding.Name);
                }

                var (beeGarden, newBeeGarden) = await GetBeeGarden(model.BeeGarden.UniqueKey, holding, newHolding, cancellationToken);
                if (!newBeeGarden)
                {
                    beeGarden.Update(model.BeeGarden.Name, model.BeeGarden.TimeZone);
                }

                var (hive, newHive) = await GetHive(model.Hive.UniqueKey, beeGarden, newBeeGarden, cancellationToken);
                if (!newHive)
                {
                    hive.Update(model.Hive.Name, model.Hive.ComPort, model.Hive.AudioSensorSampleRate, model.Hive.AudioSensorChannels, model.Hive.AudioSensorBitsPerSample);
                }

                var (state, _) = await GetExportState(Domain.BeeGardens.ExportEntity.Hive, beeGarden, newBeeGarden, cancellationToken);
                state.Update(model.CreatedOrUpdatedDate.UtcDateTime);

                await _beeHiveDbContext.SaveChangesAsync(cancellationToken);
            }
        }

        if (cmd.HiveMedia is not null)
        {
            foreach (var model in cmd.HiveMedia)
            {
                var (beeGarden, newBeeGarden) = await GetBeeGarden(model.HoldingUniqueKey, model.BeeGardenUniqueKey, cancellationToken);
                var (hive, newHive) = await GetHive(model.HiveUniqueKey, beeGarden, newBeeGarden, cancellationToken);

                var (media, newMedia) = await GetHiveMedia(model.Id, hive, newHive, cancellationToken);
                media.Update(model.Url, model.LocalPath, model.Title, model.MediaType);
                if (model.IsDeleted)
                    media.Delete();

                var (state, _) = await GetExportState(Domain.BeeGardens.ExportEntity.HiveMedia, beeGarden, newBeeGarden, cancellationToken);
                state.Update(model.CreatedOrUpdatedDate.UtcDateTime);

                await _beeHiveDbContext.SaveChangesAsync(cancellationToken);
            }
        }

        if (cmd.HiveData is not null)
        {
            foreach (var model in cmd.HiveData)
            {
                var (beeGarden, newBeeGarden) = await GetBeeGarden(model.HoldingUniqueKey, model.BeeGardenUniqueKey, cancellationToken);
                var (hive, newHive) = await GetHive(model.HiveUniqueKey, beeGarden, newBeeGarden, cancellationToken);

                var (state, _) = await GetExportState(Domain.BeeGardens.ExportEntity.HiveMedia, beeGarden, newBeeGarden, cancellationToken);

                var (timeSeries, newTimeSeries) = await GetTimeSeries(model.Kind, hive, newHive, cancellationToken);
                var (aggregateSeries, newAggregateSeries) = await GetAggregate(model.Period, timeSeries, newTimeSeries, cancellationToken);
                foreach (var stats in model.Datas)
                {
                    var timeAggregateSeriesData = await GetAggregateData(stats.Timestamp, aggregateSeries, newHive, cancellationToken);
                    if (stats.LastValueTimestamp.HasValue && stats.LastValue.HasValue)
                    {
                        var seriesData = await GetSeriesData(stats.LastValueTimestamp.Value, timeSeries, newHive, cancellationToken);
                        if (seriesData is null)
                            _beeHiveDbContext.TimeSeriesData.Add(timeSeries.AddData(stats.LastValueTimestamp.Value.UtcDateTime, stats.LastValue.Value));
                        else
                            seriesData.Value = stats.LastValue.Value;
                    }

                    if (timeAggregateSeriesData is null)
                    {
                        timeAggregateSeriesData = aggregateSeries.CreateData(stats.Timestamp.UtcDateTime,
                            stats.Count,
                            stats.MaxValue,
                            stats.MinValue,
                            stats.AvgValue,
                            stats.MedValue);
                        await _beeHiveDbContext.TimeAggregateSeriesData.AddAsync(timeAggregateSeriesData, cancellationToken);
                    }
                    else
                    {
                        aggregateSeries.UpdateData(timeAggregateSeriesData,
                            stats.Count,
                            stats.MaxValue,
                            stats.MinValue,
                            stats.AvgValue,
                            stats.MedValue);
                    }

                    state.Update(stats.CreatedOrUpdatedDate.UtcDateTime);

                    await _beeHiveDbContext.SaveChangesAsync(cancellationToken);
                }
            }
        }
    }

    public async Task<(Domain.Hives.HiveMedia state, bool newMedia)> GetHiveMedia(int orginalId, Domain.Hives.Hive hive, bool newhive, CancellationToken cancelationToken)
    {
        bool newMedia = false;
        var m = newhive ? null : await _beeHiveDbContext.HiveMedia.FirstOrDefaultAsync(x => x.OrginalId == orginalId && x.Hive.Id == hive.Id, cancelationToken);
        if (m is null)
        {
            newMedia = true;
            m = new Domain.Hives.HiveMedia(hive, orginalId);
            await _beeHiveDbContext.HiveMedia.AddAsync(m, cancelationToken);
        }

        return (m, newMedia);
    }
}