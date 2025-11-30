using Core.App.Bus;
using Core.App.Decorators;
using Core.App.Handlers;
using Core.App.Handlers.Async;
using Microsoft.Extensions.DependencyInjection;

namespace Core.App.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreAppServices(this IServiceCollection services)
    {
        return services
            .AddTransient<IQueryBus, QueryBus>()
            .AddTransient<ICommandBus, CommandBus>()
            .AddSingleton<AssemblyScanner>()
            .AddScoped<IHandlerProvider, HandlerProvider>()
            .AddScoped<IAsyncTaskSignaler, AsyncTaskSignaler>();
    }

    public static IServiceCollection AddHandlerAssembly<T>(this IServiceCollection services)
    {
        return services.AddTransient<IHandlerAssembly, HandlerAssembly<T>>();
    }

    public static IServiceCollection AddScopedWithServiceScopeWrappedDecorator<TInterface, TImplementation>(this IServiceCollection services)
        where TInterface : class
        where TImplementation : class, TInterface
    {
        services.AddScoped<TImplementation>();
        services.AddSingleton(ServiceScopeWrappedDecorator<TInterface, TImplementation>.Create);

        return services;
    }
}