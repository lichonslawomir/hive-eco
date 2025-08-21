using Core.Domain.DomainEvents;
using System.Text.Json.Serialization;

namespace BeeHive.Domain.Hives.Events;

public class AddAudioDataEvent : IDomainEvent<int>
{
    protected internal readonly Hive? _entity;
    private readonly int? _entityId;

    [JsonConstructor]
    public AddAudioDataEvent(int entityId, string fileName, bool complete)
    {
        _entityId = entityId;
        FileName = fileName;
        Complete = complete;
    }

    public AddAudioDataEvent(Hive entity, string fileName, bool complete)
    {
        _entity = entity;
        FileName = fileName;
        Complete = complete;
    }

    public int EntityId
    {
        get
        {
            if (_entity is not null)
                return _entity.Id;
            return _entityId ?? throw new NotSupportedException("Id not found");
        }
    }

    public string FileName { get; private set; }

    public bool Complete { get; private set; }
}