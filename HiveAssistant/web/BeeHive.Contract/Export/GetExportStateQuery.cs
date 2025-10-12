using BeeHive.Domain.BeeGardens;
using Core.Contract;

namespace BeeHive.Contract.Export;

public class GetExportStateQuery : IQuery<DateTimeOffset?>
{
    public required string HoldingUniqueKey { get; init; }
    public required string BeeGardenUniqueKey { get; init; }
    public ExportEntity ExportEntity { get; init; }
}