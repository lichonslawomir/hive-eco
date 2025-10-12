namespace Core.Domain.Aggregates;

public abstract class AuditableAggregateRoot<TId, TUserId> : AggregateRoot<TId>, IAuditableEntity<TId, TUserId>
{
    public DateTime CreatedDate { get; set; }
    public TUserId? CreatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public TUserId? UpdatedBy { get; set; }

    protected AuditableAggregateRoot()
    {
    }

    protected AuditableAggregateRoot(TId id) : base(id)
    {
    }
}