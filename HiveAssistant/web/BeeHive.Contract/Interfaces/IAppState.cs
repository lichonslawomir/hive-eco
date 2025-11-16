using BeeHive.Contract.Hives.Models;

namespace BeeHive.Contract.Interfaces;

public interface IAppState
{
    event Func<IList<HiveDto>, Task>? OnHiveCollectionChange;

    event Func<Task>? OnGraphDataChange;

    event Func<Task>? OnTimeSeriesAdded;

    public ValueTask StartListeningAsync();
}