namespace Core.App.Handlers.Async;

public interface IAsyncHandlerProcessor
{
    Task Signal(IEnumerable<string> asyncTaskIds);
}