using Core.Contract;
using Core.Domain.DomainEvents;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Core.App.Extensions;

public static class TypeExtensions
{
    public static IEnumerable<Type> Resolve(this Type handlerType, params Assembly[] assemblies)
    {
        foreach (var type in assemblies.SelectMany(a => a.GetTypes()))
        {
            if (!type.IsConcrete())
                continue;
            if (type.HasImplemented(handlerType))
                yield return type;
        }
    }

    public static IEnumerable<(Type interfaceType, Type serviceType)> ResolveHandlers(this Assembly assembly, params Type[] handlerTypes)
    {
        foreach (var serviceType in assembly.GetTypes())
        {
            if (!serviceType.IsConcrete() || serviceType.IsGenericType)
                continue;

            foreach (var interfaceType in serviceType.FindInterfaces((ti, o)
                             => ti.IsGenericType && !ti.IsGenericTypeDefinition && handlerTypes.Contains(ti.GetGenericTypeDefinition()),
                         null))
            {
                yield return (interfaceType, serviceType);
            }
        }
    }

    internal static bool IsConcrete(this Type type)
    {
        return !type.IsAbstract && !type.IsInterface;
    }

    internal static bool IsOpenGeneric(this Type type)
    {
        return type.IsGenericTypeDefinition && type.ContainsGenericParameters;
    }

    internal static bool HasImplemented(this Type type, Type interfaceType)
    {
        if (type.IsOpenGeneric())
        {
            var typeArguments = interfaceType.GenericTypeArguments;
            var typeConstraints = type.GetTypeInfo().GenericTypeParameters;
            if (typeArguments.Length != typeConstraints.Length)
                return false;
            return typeConstraints.All(c => typeArguments.Any(a => a.IsAssignableFrom(c)));
        }
        else
        {
            return type.FindInterfaces((ti, o) => ti == interfaceType, null).Any();
        }
    }

    public static bool IsCommandType(this Type type)
    {
        return typeof(ICommand).IsAssignableFrom(type);
    }

    public static bool IsDomainEvent(this Type type)
    {
        return typeof(IDomainEvent).IsAssignableFrom(type);
    }

    public static bool IsGenericQueryType(this Type type)
    {
        return type.GetInterfaces().Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IQuery<>));
    }

    /// <summary>
    /// Get type full name, without assembly version and other unnecessary fields
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string GetSlimName(this Type? type)
    {
        return (type?.AssemblyQualifiedName).GetSlimName();
    }

    public static string GetSlimName(this string? type)
    {
        if (string.IsNullOrEmpty(type))
            return string.Empty;
        type = Regex.Replace(type, @"Version=[^, ]+", string.Empty);
        type = Regex.Replace(type, @"Culture=[^, ]+", string.Empty);
        type = Regex.Replace(type, @"PublicKeyToken=[^, ]+", string.Empty);
        return string.Join(",", type.Replace(" ", "").Split(",", StringSplitOptions.RemoveEmptyEntries));
    }
}