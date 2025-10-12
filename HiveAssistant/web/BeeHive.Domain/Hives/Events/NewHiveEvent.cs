using Core.Domain.DomainEvents;
using System.Text.Json.Serialization;

namespace BeeHive.Domain.Hives.Events;

public class NewHiveEvent : NewEntityDomainEvent<Hive, int>
{
    [JsonConstructor]
    public NewHiveEvent(int id) : base(id)
    {
    }

    public NewHiveEvent(Hive entity) : base(entity)
    {
    }
}