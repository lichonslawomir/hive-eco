using Core.Infra.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Core.Infra.Backgrounds;

public class StartupService(IServiceProvider serviceProvider) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbInitializers = scope.ServiceProvider.GetServices<IDatabaseInitializer>();
        foreach (var dbInitializer in dbInitializers)
            await dbInitializer.InitializeAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}