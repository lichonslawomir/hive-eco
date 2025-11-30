using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Core.App.Decorators;

public class ServiceScopeWrappedDecorator<TInterface, TImplementation> : DispatchProxy
    where TInterface : notnull
    where TImplementation : TInterface
{
    private IServiceProvider? _serviceProvider;
    private IServiceProvider ServiceProvider => _serviceProvider ?? throw new NullReferenceException(nameof(_serviceProvider));

    public static TInterface Create(IServiceProvider serviceProvider)
    {
        var proxy = Create<TInterface, ServiceScopeWrappedDecorator<TInterface, TImplementation>>();
        var decorator = (proxy as ServiceScopeWrappedDecorator<TInterface, TImplementation>)!;

        decorator._serviceProvider = serviceProvider;

        return proxy;
    }

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        ArgumentNullException.ThrowIfNull(targetMethod);

        var scope = ServiceProvider.CreateScope();
        object? result = null;

        try
        {
            var target = scope.ServiceProvider.GetRequiredService<TImplementation>();
            result = targetMethod.Invoke(target, args);

            return result;
        }
        finally
        {
            // If the result is a Task, we need to wait for it to complete before disposing the scope
            if (result is Task resultTask)
            {
#pragma warning disable VSTHRD110
                resultTask.ContinueWith(_ => scope.Dispose(), TaskScheduler.Current);
#pragma warning restore VSTHRD110
            }
            else
            {
                // Dispose the scope if the result is not a Task, otherwise it will be disposed in the continuation
                scope.Dispose();
            }
        }
    }
}

public class ServiceScopeWrappedDecorator<T> : ServiceScopeWrappedDecorator<T, T>
    where T : notnull;