namespace Hive.Gateway.Service.Services;

public interface IScopeService
{
    Task RunInScope<TService>(Func<TService, ValueTask> func) where TService : notnull;
}

public sealed class ScopeService(IServiceScopeFactory scopeFactory) : IScopeService
{
    public async Task RunInScope<TService>(Func<TService, ValueTask> func) where TService : notnull
    {
        using var scope = scopeFactory.CreateScope();
        await func(scope.ServiceProvider.GetRequiredService<TService>());
    }
}