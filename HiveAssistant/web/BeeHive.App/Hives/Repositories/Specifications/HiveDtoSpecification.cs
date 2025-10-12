using System.Linq.Expressions;
using BeeHive.Contract.Hives.Models;
using BeeHive.Domain.Hives;
using Core.App.Repositories;

namespace BeeHive.App.Hives.Repositories.Specifications;

public class HiveDtoSpecification : HiveSpecification, IPagedSpecification<Hive, HiveDto>
{
    public bool Distinct => false;

    public int? Skip { get; set; }
    public int? Take { get; set; }

    public Expression<Func<Hive, HiveDto>> Selector => HiveMappingExtensions.Map;
}