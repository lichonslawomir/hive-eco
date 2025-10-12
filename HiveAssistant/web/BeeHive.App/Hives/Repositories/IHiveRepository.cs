using BeeHive.Domain.Hives;
using Core.App.Repositories;

namespace BeeHive.App.Hives.Repositories;

public interface IHiveRepository : IGenericRepository<Hive, int>
{
}