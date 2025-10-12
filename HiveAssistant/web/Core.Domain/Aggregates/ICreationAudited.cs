namespace Core.Domain.Aggregates;

public interface ICreationAudited<TUserId>
{
    DateTime CreatedDate { get; }
    TUserId? CreatedBy { get; }
}