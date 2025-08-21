using Core.Domain.DomainEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BeeHive.Domain.Holdings.Events;

public class NewHoldingEvent : NewEntityDomainEvent<Holding, int>
{
    [JsonConstructor]
    public NewHoldingEvent(int id) : base(id)
    {
    }

    public NewHoldingEvent(Holding entity) : base(entity)
    {
    }
}