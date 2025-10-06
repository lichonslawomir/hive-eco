namespace BeeHive.Domain.BeeGardens;

public sealed class BeeGardenImportState
{
    public BeeGardenImportState(BeeGarden beeGarden, ExportEntity exportEntity)
    {
        BeeGarden = beeGarden;
        ExportEntity = exportEntity;
        LastCreatedOrUpdatedDate = DateTime.MinValue;
    }

    private BeeGardenImportState()
    {
    }

    public BeeGarden BeeGarden { get; private set; } = null!;
    public int BeeGardenId { get; private set; }

    public ExportEntity ExportEntity { get; private set; }

    public DateTime LastCreatedOrUpdatedDate { get; set; }

    public void Update(DateTime ts)
    {
        if (ts > LastCreatedOrUpdatedDate)
            LastCreatedOrUpdatedDate = ts;
    }
}