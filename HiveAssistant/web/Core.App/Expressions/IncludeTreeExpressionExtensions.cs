using Core.App.Helpers;
using Core.App.Repositories.Include;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Core.App.Expressions;

public static class IncludeTreeExpressionExtensions
{
    public static IQueryable<T> ApplyInclude<T>(this IQueryable<T> query, IncludeTreeExpression<T> includeTree)
        where T : class
    {
        var includeMethod =
            ReflectionCache
                .GetOrAddMethod(
                    typeof(EntityFrameworkQueryableExtensions),
                    nameof(EntityFrameworkQueryableExtensions.Include),
                    (type, methodName) =>
                        type.GetMethods()
                            .First(m => m.Name == methodName &&
                                        m.GetParameters().Length == 2 &&
                                        m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>)))
                .GetOrAddGenericMethod(typeof(T), includeTree.ParameterType);
        var result = includeMethod.Invoke(
            null,
            [query, includeTree.IncludeExpression]);

        foreach (var child in includeTree.Children)
        {
            result = ApplyThenInclude((IQueryable<T>)result!, child);
        }

        return (IQueryable<T>)result!;
    }

    private static IQueryable<T> ApplyThenInclude<T>(
        IQueryable<T> query,
        IIncludeTreeNodeExpression childNode)
        where T : class
    {
        var includeMethod =
            ReflectionCache
                .GetOrAddMethod(
                    typeof(EntityFrameworkQueryableExtensions),
                    nameof(EntityFrameworkQueryableExtensions.ThenInclude),
                    (type, methodName) =>
                        type.GetMethods()
                            .First(m => m.Name == methodName &&
                                        m.GetParameters().Length == 2 &&
                                        m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>)))
                .GetOrAddGenericMethod(typeof(T), childNode.EntityType, childNode.ParameterType);
        var result = includeMethod.Invoke(
            null,
            [query, childNode.IncludeExpression]);

        foreach (var grandChild in childNode.Children)
        {
            result = ApplyThenInclude((IQueryable<T>)result!, grandChild);
        }

        return (IQueryable<T>)result!;
    }
}