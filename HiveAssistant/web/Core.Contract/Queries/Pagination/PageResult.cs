using System.ComponentModel.DataAnnotations;

namespace Core.Contract.Queries.Pagination;

public class PageResult<T>
{
    [Required]
    public IReadOnlyCollection<T> Items { get; }

    public int? Limit { get; set; }

    [Required]
    public int Offset { get; set; }

    [Required]
    public long Total { get; set; }

    public PageResult(IReadOnlyCollection<T> results, long total, int offset, int? limit)
    {
        Items = results;
        Total = total;
        Offset = offset;
        Limit = limit;
    }

    public PageResult(IReadOnlyCollection<T> results)
    {
        Items = results;
        Total = results.Count;
        Offset = 0;
        Limit = null;
    }

    public PageResult<TDto> TransformResults<TDto>(Func<T, TDto> dtoTranslator) =>
        new(Items.Select(dtoTranslator).ToArray(), Total, Offset, Limit);
}