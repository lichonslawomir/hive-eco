using BeeHive.App.Aggregate.Repositories;
using BeeHive.App.Aggregate.Repositories.Specifications;
using BeeHive.App.Data.Repositories;
using BeeHive.App.Data.Repositories.Specifications;
using BeeHive.App.Hives.Repositories;
using BeeHive.App.Hives.Repositories.Specifications;
using BeeHive.App.Hives.Repositories.Specifications.Export;
using BeeHive.Contract.Export;
using BeeHive.Domain.Aggregate.Extensions;
using BeeHive.Domain.BeeGardens;
using Core.App.Extensions;
using Core.App.Repositories;
using Core.Contract.Schedule;

namespace Hive.Gateway.Service.Export;

public class ExportJob(
    IHiveRepository hiveRepository,
    IHiveMediaRepository hiveMediaRepository,
    ITimeAggregateSeriesDataRepository timeAggregateSeriesDataRepository,
    ITimeSeriesDataRepository timeSeriesDataRepository,
    IExportService exportService) : IJob
{
    private const int ExportRange = 100;

    public static ExecuteConfig DefaultExecuteConfig = new()
    {
        Period = TimeSpan.FromSeconds(20),
        MaxExecuteTime = TimeSpan.FromMinutes(50),
        Queue = "Export"
    };

    public async Task Execute(CancellationToken stoppingToken)
    {
        var ts = await exportService.GetLastExportDate(ExportEntity.Hive, stoppingToken);
        bool go = false;
        do
        {
            var specifications = new HiveExportSpecification()
            {
                CreatedOrUpdatedDate = ts.HasValue ? ts.Value.UtcDateTime : null,
                Take = ExportRange
            };
            var result = await hiveRepository.GetPagedAsync(specifications, stoppingToken);
            if (result.Items.Any())
            {
                var nextTs = result.Items.Last().CreatedOrUpdatedDate;
                await exportService.Export(result.Items, nextTs, stoppingToken);
                ts = nextTs;
            }
            go = result.Items.Count >= ExportRange;
        }
        while (go);

        ts = await exportService.GetLastExportDate(ExportEntity.HiveMedia, stoppingToken);
        do
        {
            var specifications = new HiveMediaExportSpecification()
            {
                CreatedOrUpdatedDate = ts.HasValue ? ts.Value.UtcDateTime : null,
                Take = ExportRange,
            };
            var result = await hiveMediaRepository.GetPagedAsync(specifications, stoppingToken);
            if (result.Items.Any())
            {
                var nextTs = result.Items.Last().CreatedOrUpdatedDate;
                await exportService.Export(result.Items, nextTs, stoppingToken);
                ts = nextTs;
            }
            go = result.Items.Count >= ExportRange;
        }
        while (go);

        ts = await exportService.GetLastExportDate(ExportEntity.TimeTimeAggregateSeriesData, stoppingToken);
        do
        {
            var specifications = new TimeAggregateSeriesHivesDataSpecification()
            {
                CreatedOrUpdatedDate = ts.HasValue ? ts.Value.UtcDateTime : null,
                Ordering = HiveOrdering.CreatedOrUpdatedDateAsc
            };
            var result = await timeAggregateSeriesDataRepository.GetPagedAsync(specifications, 0, ExportRange, stoppingToken);
            if (result.Items.Any())
            {
                //TimeAggregateSeriesExportModel
                var exportItems = result.Items.GroupBy(x => new { x.HiveId, x.Kind, x.Period })
                    .Select(async x =>
                    {
                        var ex = await hiveRepository.GetByIdAsync(x.Key.HiveId, MapExportExtensions.MapExport, stoppingToken);
                        return new TimeAggregateSeriesExportModel()
                        {
                            HoldingUniqueKey = ex.BeeGarden.UniqueKey,
                            BeeGardenUniqueKey = ex.BeeGarden.UniqueKey,
                            HiveUniqueKey = ex.Hive.UniqueKey,
                            Kind = x.Key.Kind,
                            Period = x.Key.Period,
                            Datas = x.Select(async y =>
                            {
                                var range = y.Timestamp.GetRangeEnd(x.Key.Period);
                                var lv = await timeSeriesDataRepository.GetSingleAsync(new TimeSeriesDataSpecification()
                                {
                                    HiveId = x.Key.HiveId,
                                    Kind = x.Key.Kind,
                                    From = y.Timestamp.UtcDateTime,
                                    To = range.UtcDateTime,
                                }, stoppingToken);
                                return new TimeAggregateSeriesExportModel.Data
                                {
                                    Timestamp = y.Timestamp,
                                    Count = y.Count,
                                    MaxValue = y.MaxValue,
                                    MinValue = y.MinValue,
                                    AvgValue = y.AvgValue,
                                    MedValue = y.MedValue,
                                    LastValue = lv.Value,
                                    LastValueTimestamp = lv.Timestamp,
                                    CreatedOrUpdatedDate = y.CreatedOrUpdatedDate
                                };
                            }).Select(x => x.Result).ToList()
                        };
                    }).Select(x => x.Result).ToList();
                var nextTs = result.Items.Last().CreatedOrUpdatedDate;
                await exportService.Export(exportItems, nextTs, stoppingToken);
                ts = nextTs;
            }
            go = result.Items.Count >= ExportRange;
        }
        while (go);
    }
}