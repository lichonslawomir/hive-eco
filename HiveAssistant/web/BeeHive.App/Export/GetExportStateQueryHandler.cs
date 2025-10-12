using BeeHive.Contract.Export;
using Core.App;
using Core.App.Handlers;
using Microsoft.EntityFrameworkCore;

namespace BeeHive.App.Export;

public class GetExportStateQueryHandler : BaseDataExportService, IQueryHandler<GetExportStateQuery, DateTimeOffset?>
{
    public GetExportStateQueryHandler(IBeeHiveDbContext beeHiveDbContext, IWorkContext workContext) : base(beeHiveDbContext, workContext)
    {
    }

    public async Task<DateTimeOffset?> HandleQuery(GetExportStateQuery query, CancellationToken cancellationToken)
    {
        var state = await _beeHiveDbContext.BeeGardenImportStates
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ExportEntity == query.ExportEntity &&
                x.BeeGarden.Holding.UniqueKey == query.HoldingUniqueKey &&
                x.BeeGarden.UniqueKey == query.BeeGardenUniqueKey, cancellationToken);
        return state?.LastCreatedOrUpdatedDate;
    }
}