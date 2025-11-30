using BeeHive.App;
using BeeHive.Infra.DataAccess.DbContexts;
using BeeHive.Infra.Sqlite.Services;
using Core.Infra.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BeeHive.Infra.Sqlite.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBeeHiveDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(nameof(BeeHiveDbContext));
        services.AddDbContext<BeeHiveDbContext>(options =>
        {
            options.UseSqlite(connectionString, s => s.MigrationsAssembly("BeeHive.Infra.Sqlite"));
        });
        services.AddScoped<IBeeHiveDbContext>(provider => provider.GetRequiredService<BeeHiveDbContext>())
            .AddScoped<IDatabaseInitializer, DatabaseInitializer>()
            .AddScoped<IBeeHiveDbContextConfigurationProvider, BeeHiveDbContextConfigurationProvider>();

        return services;
    }
}