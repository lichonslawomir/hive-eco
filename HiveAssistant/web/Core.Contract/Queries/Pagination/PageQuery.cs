namespace Core.Contract.Queries.Pagination;

public abstract record PageQuery<T> : IQuery<PageResult<T>>
{
    /// <summary>
    /// Take {limit} items
    /// </summary>
    public int? Limit { get; set; } = 10;

    /// <summary>
    /// Skip {offset} items
    /// </summary>
    public int Offset { get; set; } = 0;
}