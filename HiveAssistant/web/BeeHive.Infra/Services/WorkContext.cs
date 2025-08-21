using Core.App;

namespace BeeHive.Infra.Services;

public sealed class WorkContext : WorkContext<string>
{
    public override TUserId? GetUserId<TUserId>() where TUserId : default
    {
        if (User is TUserId)
            return (TUserId)(object)User;

        return default(TUserId);
    }
}