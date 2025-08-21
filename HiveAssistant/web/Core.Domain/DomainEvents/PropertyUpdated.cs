namespace Core.Domain.DomainEvents;

public struct PropertyUpdated<TProperty>
{
    public PropertyUpdated(TProperty oldValue, TProperty newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }

    public TProperty NewValue { get; private set; }

    public TProperty OldValue { get; private set; }
}