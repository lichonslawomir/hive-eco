namespace Core.App.Repositories.Include;

public interface IIncludeTreeNodeExpression
{
    internal Type EntityType { get; }
    internal Type ParameterType { get; }
    internal object IncludeExpression { get; }
    internal IEnumerable<IIncludeTreeNodeExpression> Children { get; }
}