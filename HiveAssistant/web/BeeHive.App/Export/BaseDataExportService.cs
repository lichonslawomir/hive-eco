using BeeHive.App.Sensors;
using Core.App;
using Microsoft.EntityFrameworkCore;

namespace BeeHive.App.Export;

public abstract class BaseDataExportService : BaseDataService
{
    protected BaseDataExportService(IBeeHiveDbContext beeHiveDbContext, IWorkContext workContext) : base(beeHiveDbContext, workContext)
    {
    }

    protected async Task<(Domain.BeeGardens.BeeGardenImportState state, bool newState)> GetExportState(Domain.BeeGardens.ExportEntity exportEntity, Domain.BeeGardens.BeeGarden beeGarden, bool newBeeGarden, CancellationToken cancelationToken)
    {
        bool newState = false;
        var state = newBeeGarden ? null : await _beeHiveDbContext.BeeGardenImportStates.FirstOrDefaultAsync(x => x.ExportEntity == exportEntity && x.BeeGardenId == beeGarden.Id, cancelationToken);
        if (state is null)
        {
            newState = true;
            state = new Domain.BeeGardens.BeeGardenImportState(beeGarden, exportEntity);
            await _beeHiveDbContext.BeeGardenImportStates.AddAsync(state, cancelationToken);
        }

        return (state, newState);
    }
}
