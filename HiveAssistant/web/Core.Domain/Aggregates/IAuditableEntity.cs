namespace Core.Domain.Aggregates;

public interface IAuditableEntity<TUserId>
{
    DateTime? UpdatedDate { get; }
    TUserId? UpdatedBy { get; }
}

public interface IAuditableEntity<TId, TUserId> : IEntity<TId>, IAuditableEntity<TUserId>, ICreationAudited<TUserId>
{
}