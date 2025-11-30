using BeeHive.Infra.Services;
using Core.App;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EfMigrationTools;

public abstract class DesignTimeDbContextFactory<TContext> : IDesignTimeDbContextFactory<TContext> where TContext : DbContext
{
    public TContext CreateDbContext(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{environment}.json")
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();

        var services = new ServiceCollection()
            .AddScoped<IWorkContext, WorkContext>();

        AddDbContext(services, configuration);

        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<TContext>();
    }

    protected abstract void AddDbContext(IServiceCollection services, IConfiguration configuration);
}