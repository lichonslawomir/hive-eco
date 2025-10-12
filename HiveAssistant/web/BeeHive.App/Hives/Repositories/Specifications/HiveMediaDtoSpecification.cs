using System.Linq.Expressions;
using BeeHive.Contract.Hives.Models;
using BeeHive.Domain.Hives;
using Core.App.Repositories;

namespace BeeHive.App.Hives.Repositories.Specifications;

public class HiveMediaDtoSpecification : HiveMediaSpecification, IMapSpecification<HiveMedia, HiveMediaDto>
{
    public bool Distinct => false;

    public Expression<Func<HiveMedia, HiveMediaDto>> Selector => HiveMappingExtensions.MapMedia;
}