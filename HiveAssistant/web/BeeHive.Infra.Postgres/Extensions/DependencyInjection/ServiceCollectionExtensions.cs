using BeeHive.App;
using BeeHive.Infra.DataAccess.DbContexts;
using BeeHive.Infra.Postgres.Services;
using Core.Infra.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace BeeHive.Infra.Postgres.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBeeHiveDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(nameof(BeeHiveDbContext));
        var csBuilder = new NpgsqlConnectionStringBuilder(connectionString);

        var host = configuration["DATABASE_HOST"];
        if (!string.IsNullOrEmpty(host))
            csBuilder.Host = host;

        var user = configuration["DATABASE_USER"];
        if (!string.IsNullOrEmpty(user))
            csBuilder.Username = user;

        var paswd = configuration["DATABASE_PASSWORD"];
        if (!string.IsNullOrEmpty(paswd))
            csBuilder.Password = paswd;

        var dbname = configuration["DATABASE_NAME"];
        if (!string.IsNullOrEmpty(dbname))
            csBuilder.Database = dbname;

        services.AddDbContext<BeeHiveDbContext>(options =>
            options.UseNpgsql(csBuilder.ConnectionString, s => s.MigrationsAssembly("BeeHive.Infra.Postgres"))
        );
        services.AddScoped<IBeeHiveDbContext>(provider => provider.GetRequiredService<BeeHiveDbContext>())
            .AddScoped<IDatabaseInitializer, DatabaseInitializer>()
            .AddScoped<IBeeHiveDbContextConfigurationProvider, BeeHiveDbContextConfigurationProvider>();

        return services;
    }
}