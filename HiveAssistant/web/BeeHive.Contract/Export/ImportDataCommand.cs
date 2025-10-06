using Core.Contract;

namespace BeeHive.Contract.Export;

public class ImportDataCommand : ICommand
{
    public HiveExportModel[]? Hives { get; init; }
    public HiveMediaExportModel[]? HiveMedia { get; init; }
    public TimeAggregateSeriesExportModel[]? HiveData { get; init; }
}