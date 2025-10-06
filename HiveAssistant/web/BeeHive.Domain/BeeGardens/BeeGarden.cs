using BeeHive.Domain.BeeGardens.Events;
using BeeHive.Domain.Holdings;
using Core.Domain.Aggregates;

namespace BeeHive.Domain.BeeGardens;

public sealed class BeeGarden : AuditableAggregateRoot<int, string>
{
    public string UniqueKey { get; private set; }
    public string Name { get; private set; }

    public string TimeZone { get; private set; }

    public Holding Holding { get; private set; } = null!;
    public int HoldingId { get; private set; }

    private BeeGarden(string name, string uniqueKey, string timeZone)
    {
        Name = name;
        UniqueKey = uniqueKey;
        TimeZone = timeZone;
    }

    public static BeeGarden Create(string name, string uniqueKey, string timeZone, Holding holding)
    {
        var beeGarden = new BeeGarden(name, uniqueKey, timeZone)
        {
            Holding = holding
        };

        beeGarden.PublishEvent(new NewBeeGardenEvent(beeGarden));

        return beeGarden;
    }

    public void Update(string name, string timeZone)
    {
        this.TimeZone = timeZone;
        this.Name = name;
    }
}