using BeeHive.Contract.Hives;
using BeeHive.Domain.Hives;
using System.Linq.Expressions;

namespace BeeHive.App.Hives.Repositories.Specifications;

public static class MappingExtensions
{
    public static HiveDto ToDto(this Hive e)
    {
        return new HiveDto()
        {
            Id = e.Id,
            Name = e.Name,
            UniqueKey = e.UniqueKey
        };
    }

    public static Expression<Func<Hive, HiveDto>> Map = e => new HiveDto()
    {
        Id = e.Id,
        Name = e.Name,
        UniqueKey = e.UniqueKey
    };
}