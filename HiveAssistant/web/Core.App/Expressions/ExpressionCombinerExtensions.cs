using System.Linq.Expressions;

namespace Core.App.Expressions;

public static class ExpressionCombinerExtensions
{
    public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> first,
        Expression<Func<T, bool>> second)
    {
        var parameter = Expression.Parameter(typeof(T));

        var visitor = new ReplaceParameterVisitor();
        visitor.Add(first.Parameters[0], parameter);
        visitor.Add(second.Parameters[0], parameter);

        var combined = visitor.Visit(Expression.AndAlso(first.Body, second.Body));

        return Expression.Lambda<Func<T, bool>>(combined, parameter);
    }

    public static Expression<Func<T, bool>> AndAlso<T>(this IEnumerable<Expression<Func<T, bool>>> filters)
    {
        Expression<Func<T, bool>>? combinedExpression = null;
        foreach (var filter in filters)
        {
            combinedExpression = combinedExpression is null ? filter : combinedExpression.AndAlso(filter);
        }
        if (combinedExpression is null)
            return x => true;
        return combinedExpression;
    }

    public static Expression<Func<T, bool>> OrAlso<T>(this Expression<Func<T, bool>> first,
        Expression<Func<T, bool>> second)
    {
        var parameter = Expression.Parameter(typeof(T));

        var visitor = new ReplaceParameterVisitor();
        visitor.Add(first.Parameters[0], parameter);
        visitor.Add(second.Parameters[0], parameter);

        var combined = visitor.Visit(Expression.OrElse(first.Body, second.Body));

        return Expression.Lambda<Func<T, bool>>(combined, parameter);
    }

    public static Expression<Func<T, bool>> OrAlso<T>(this IEnumerable<Expression<Func<T, bool>>> filters)
    {
        Expression<Func<T, bool>>? combinedExpression = null;
        foreach (var filter in filters)
        {
            combinedExpression = combinedExpression is null ? filter : combinedExpression.OrAlso(filter);
        }
        if (combinedExpression is null)
            return x => true;
        return combinedExpression;
    }

    #region Private

    private class ReplaceParameterVisitor : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> _map = new();

        public void Add(ParameterExpression from, ParameterExpression to)
        {
            _map[from] = to;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (_map.TryGetValue(node, out var replacement))
            {
                node = replacement;
            }
            return base.VisitParameter(node);
        }
    }

    #endregion Private
}