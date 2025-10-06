using System.Reflection;

namespace Core.App.Handlers;

internal interface IHandlerAssembly
{
    Assembly GetAssembly();
}

internal sealed class HandlerAssembly<TMarker> : IHandlerAssembly
{
    public Assembly GetAssembly()
    {
        return typeof(TMarker).Assembly;
    }
}