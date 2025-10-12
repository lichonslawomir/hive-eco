using Core.App.DataAccess;

namespace Core.App.Extensions;

public static class CommitDelegateExtensions
{
    public static async Task InvokeAsync(this CommitDelegate? handlers, CancellationToken cancellationToken = default)
    {
        if (handlers is not null)
        {
            var individualHandlers = handlers.GetInvocationList();
            List<Exception>? exceptions = null;
            foreach (var @delegate in individualHandlers)
            {
                var handler = (CommitDelegate)@delegate;
                try
                {
                    await handler(cancellationToken).ConfigureAwait(true);
                }
                catch (Exception ex)
                {
                    exceptions ??= new List<Exception>(2);
                    exceptions.Add(ex);
                }
            }

            if (exceptions is not null)
            {
                throw new AggregateException(exceptions);
            }
        }
    }
}