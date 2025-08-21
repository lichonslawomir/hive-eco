using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Reflection;

namespace Core.App.Helpers;

public static class ReflectionCache
{
    private static readonly ConcurrentDictionary<(Type DeclaringType, string MethodName), MethodInfo> MethodCache = new();

    private static readonly ConcurrentDictionary<(MethodInfo Method, ImmutableArray<Type> GenericArguments), MethodInfo> GenericMethodCache = new();

    public static MethodInfo GetOrAddMethod(Type declaringType, string methodName, Func<Type, string, MethodInfo>? valueFactory)
    {
        valueFactory ??= (t, m) =>
        {
            var method = t.GetMethod(m);
            if (method == null)
            {
                throw new MissingMethodException(declaringType.FullName, methodName);
            }
            return method;
        };
        return MethodCache.GetOrAdd((declaringType, methodName), _ => valueFactory(declaringType, methodName));
    }

    public static MethodInfo GetOrAddMethod<T>(string methodName, Func<Type, string, MethodInfo>? valueFactory = null)
    {
        return GetOrAddMethod(typeof(T), methodName, valueFactory);
    }

    public static MethodInfo GetOrAddGenericMethod(this MethodInfo method, params Type[] genericArguments)
    {
        return GenericMethodCache.GetOrAdd((method, [.. genericArguments]), key => method.MakeGenericMethod(genericArguments));
    }
}