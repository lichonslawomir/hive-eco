using BeeHive.Domain.Holdings.Events;
using Core.Domain.Aggregates;

namespace BeeHive.Domain.Holdings;

public class Holding : AuditableAggregateRoot<int, string>
{
    public string Name { get; protected set; } = string.Empty;
    public string UniqueKey { get; protected set; } = string.Empty;

    public static Holding Create(string name, string uniqueKey)
    {
        var holding = new Holding()
        {
            Name = name,
            UniqueKey = uniqueKey
        };
        holding.PublishEvent(new NewHoldingEvent(holding));

        return holding;
    }
}