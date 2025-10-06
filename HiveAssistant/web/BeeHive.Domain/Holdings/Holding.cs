using BeeHive.Domain.Holdings.Events;
using Core.Domain.Aggregates;

namespace BeeHive.Domain.Holdings;

public sealed class Holding : AuditableAggregateRoot<int, string>
{
    public string Name { get; private set; }
    public string UniqueKey { get; private set; }

    private Holding(string name, string uniqueKey)
    {
        Name = name;
        UniqueKey = uniqueKey;
    }

    public static Holding Create(string name, string uniqueKey)
    {
        var holding = new Holding(name, uniqueKey);
        holding.PublishEvent(new NewHoldingEvent(holding));

        return holding;
    }

    public void Update(string name)
    {
        this.Name = name;
    }
}