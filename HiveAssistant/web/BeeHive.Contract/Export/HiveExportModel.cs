using BeeHive.Contract.BeeGardens.Models;
using BeeHive.Contract.Hives.Models;
using BeeHive.Contract.Holdings.Models;

namespace BeeHive.Contract.Export;

public struct HiveExportModel
{
    public required HoldingDto Holding { get; init; }
    public required BeeGardenDto BeeGarden { get; init; }
    public required HiveDto Hive { get; init; }

    public DateTimeOffset CreatedOrUpdatedDate { get; init; }
}