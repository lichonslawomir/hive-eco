using BeeHive.Contract.Hives.Models;
using Core.App.DataAccess;

namespace Hive.Gateway.Service.Services;

public class AppState
{
    public event Func<IList<HiveDto>, Task>? OnHiveCollectionChange;

    public event Func<Task>? OnGraphDataChange;

    public event Func<Task>? OnTimeSeriesAdded;

    public async Task NotifyHiveCollectionChange(IList<HiveDto> hives)
    {
        if (OnHiveCollectionChange is null)
            return;

        var individualHandlers = OnHiveCollectionChange.GetInvocationList();
        foreach (var @delegate in individualHandlers)
        {
            var handler = (Func<IList<HiveDto>, Task>)@delegate;
            await handler(hives).ConfigureAwait(true);
        }
    }

    public async Task NotifyGraphDataChange()
    {
        if (OnGraphDataChange is null)
            return;

        var individualHandlers = OnGraphDataChange.GetInvocationList();
        foreach (var @delegate in individualHandlers)
        {
            var handler = (Func<Task>)@delegate;
            await handler().ConfigureAwait(true);
        }
    }

    public async Task NotifyTimeSeriesAdded()
    {
        if (OnTimeSeriesAdded is null)
            return;

        var individualHandlers = OnTimeSeriesAdded.GetInvocationList();
        foreach (var @delegate in individualHandlers)
        {
            var handler = (Func<Task>)@delegate;
            await handler().ConfigureAwait(true);
        }
    }
}