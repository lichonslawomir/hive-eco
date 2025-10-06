using BeeHive.Contract.Hives.Models;

namespace Hive.Gateway.Service.Services;

public class AppState
{
    public event Func<IList<HiveDto>, Task>? OnHiveCollectionChange;

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
}