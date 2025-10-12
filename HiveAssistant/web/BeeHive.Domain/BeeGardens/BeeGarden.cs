using BeeHive.Domain.BeeGardens.Events;
using BeeHive.Domain.Holdings;
using Core.Domain.Aggregates;

namespace BeeHive.Domain.BeeGardens;

public class BeeGarden : AuditableAggregateRoot<int, string>
{
    public string UniqueKey { get; protected set; }
    public string Name { get; protected set; }

    public string TimeZone { get; protected set; }

    public Holding Holding { get; protected set; } = null!;
    public int HoldingId { get; protected set; }

    protected BeeGarden(string name, string uniqueKey, string timeZone)
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
}