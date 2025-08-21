using Core.Domain.DomainEvents;
using System.Text.Json.Serialization;

namespace BeeHive.Domain.BeeGardens.Events;

public class NewBeeGardenEvent : NewEntityDomainEvent<BeeGarden, int>
{
    [JsonConstructor]
    public NewBeeGardenEvent(int id) : base(id)
    {
    }

    public NewBeeGardenEvent(BeeGarden entity) : base(entity)
    {
    }
}