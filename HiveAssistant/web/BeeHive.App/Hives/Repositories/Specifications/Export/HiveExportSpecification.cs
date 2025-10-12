using BeeHive.App.Hives.Repositories.Specifications.Order;
using BeeHive.Contract.Export;
using BeeHive.Domain.Hives;
using Core.App.Repositories;
using Core.App.Repositories.Filter;
using Core.App.Repositories.Order;
using System.Linq.Expressions;

namespace BeeHive.App.Hives.Repositories.Specifications.Export;

public class HiveExportSpecification : IPagedSpecification<Hive, HiveExportModel>
{
    public bool Distinct => false;

    public int? Skip { get; set; }
    public required int? Take { get; set; }

    public required DateTime? CreatedOrUpdatedDate { get; set; }

    public IEnumerable<IFilter<Hive>> AsEnumerableFilters()
    {
        if (CreatedOrUpdatedDate.HasValue)
            yield return new CreatedOrUpdatedDateFilter<Hive>(CreatedOrUpdatedDate.Value);

        yield break;
    }

    public IOrder<Hive>? OrderBy()
    {
        return new CreatedOrUpdatedDateOrder<Hive>(true);
    }

    public Expression<Func<Hive, HiveExportModel>> Selector => MapExportExtensions.MapExport;
}