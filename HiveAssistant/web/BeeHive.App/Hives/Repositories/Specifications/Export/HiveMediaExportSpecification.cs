using System.Linq.Expressions;
using BeeHive.App.Hives.Repositories.Specifications.Order;
using BeeHive.Contract.Export;
using BeeHive.Domain.Hives;
using Core.App.Repositories;
using Core.App.Repositories.Filter;
using Core.App.Repositories.Order;

namespace BeeHive.App.Hives.Repositories.Specifications.Export;

public class HiveMediaExportSpecification : IPagedSpecification<HiveMedia, HiveMediaExportModel>
{
    public bool Distinct => false;

    public int? Skip { get; set; }
    public required int? Take { get; set; }

    public required DateTime? CreatedOrUpdatedDate { get; set; }

    public IEnumerable<IFilter<HiveMedia>> AsEnumerableFilters()
    {
        if (CreatedOrUpdatedDate.HasValue)
            yield return new CreatedOrUpdatedDateFilter<HiveMedia>(CreatedOrUpdatedDate.Value);

        yield break;
    }

    public IOrder<HiveMedia>? OrderBy()
    {
        return new CreatedOrUpdatedDateOrder<HiveMedia>(true);
    }

    public Expression<Func<HiveMedia, HiveMediaExportModel>> Selector => MapExportExtensions.MapMediaExport;
}