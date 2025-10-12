using BeeHive.Domain.Data.Events;
using Core.App;
using Core.App.Handlers;

namespace Hive.Gateway.Service.DomainEvents;

public sealed class AddTimeSeriesDataEventHandler(ICommandBus commandBus) : IDomainEventHandler<AddTimeSeriesDataEvent>
{
    private bool _alreadySet = false;

    public int Order => 0;

    public async ValueTask HandleEvent(AddTimeSeriesDataEvent e, CancellationToken cancellationToken)
    {
        if (_alreadySet)
            return;
        _alreadySet = true;
        await commandBus.ExecuteCommand(new RefreshAppStateCommand()
        {
            RefreshAppState = true
        }, cancellationToken);
    }
}