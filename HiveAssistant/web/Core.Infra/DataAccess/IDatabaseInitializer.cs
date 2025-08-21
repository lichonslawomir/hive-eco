namespace Core.Infra.DataAccess;

public interface IDatabaseInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken);
}