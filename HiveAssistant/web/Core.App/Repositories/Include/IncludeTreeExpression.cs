using System.Linq.Expressions;

namespace Core.App.Repositories.Include;

public abstract class IncludeTreeExpression<T> : IIncludeTreeNodeExpression
    where T : class
{
    private List<IIncludeTreeNodeExpression>? _childNodes;

    public Type EntityType => typeof(T);
    public abstract Type ParameterType { get; }
    public object IncludeExpression { get; }
    public IEnumerable<IIncludeTreeNodeExpression> Children => _childNodes?.AsEnumerable() ?? Array.Empty<IIncludeTreeNodeExpression>();

    protected IncludeTreeExpression(object includeExpression)
    {
        IncludeExpression = includeExpression;
    }

    protected internal void AddChild(IIncludeTreeNodeExpression child)
    {
        _childNodes ??= new List<IIncludeTreeNodeExpression>();
        _childNodes.Add(child);
    }
}

public class IncludeSingle<T, TKey> : IncludeTreeExpression<T>
    where T : class
    where TKey : class
{
    public override Type ParameterType => typeof(TKey);

    public IncludeSingle(Expression<Func<T, TKey>> includeExpression)
        : base(includeExpression)
    {
    }

    public IncludeTreeExpression<T> ThenInclude<TKeyKey>(Expression<Func<TKey, TKeyKey>> thenInclude)
        where TKeyKey : class
    {
        AddChild(new IncludeSingle<TKey, TKeyKey>(thenInclude));
        return this;
    }

    public IncludeTreeExpression<T> ThenInclude<TKeyKey>(Expression<Func<TKey, IEnumerable<TKeyKey>>> thenInclude)
        where TKeyKey : class
    {
        AddChild(new IncludeMany<TKey, TKeyKey>((object)thenInclude, thenInclude.ReturnType));
        return this;
    }
}

public class IncludeMany<T, TKey> : IncludeTreeExpression<T>
    where T : class
    where TKey : class
{
    public override Type ParameterType { get; }

    public IncludeMany(Expression<Func<T, IEnumerable<TKey>>> includeExpression)
        : base(includeExpression)
    {
        ParameterType = includeExpression.ReturnType;
    }

    internal IncludeMany(object includeExpression, Type returnType)
        : base(includeExpression)
    {
        ParameterType = returnType;
    }

    public IncludeTreeExpression<T> ThenInclude<TKeyKey>(Expression<Func<TKey, TKeyKey>> thenInclude)
        where TKeyKey : class
    {
        AddChild(new IncludeMany<TKey, TKeyKey>(thenInclude, thenInclude.ReturnType));
        return this;
    }

    public IncludeTreeExpression<T> ThenInclude<TKeyKey>(Expression<Func<TKey, IEnumerable<TKeyKey>>> thenInclude)
        where TKeyKey : class
    {
        AddChild(new IncludeMany<TKey, TKeyKey>((object)thenInclude, thenInclude.ReturnType));
        return this;
    }
}