using BeeHive.Domain.Aggregate.Events;
using Core.App;
using Core.App.Handlers;

namespace Hive.Gateway.Service.DomainEvents;

public sealed class UpdateTimeAggregateSeriesDataEventHandler(ICommandBus commandBus) : IDomainEventHandler<UpdateTimeAggregateSeriesDataEvent>
{
    private bool _alreadySet = false;

    public int Order => 0;

    public async ValueTask HandleEvent(UpdateTimeAggregateSeriesDataEvent e, CancellationToken cancellationToken)
    {
        if (_alreadySet)
            return;
        _alreadySet = true;
        await commandBus.ExecuteCommand(new RefreshAppStateCommand()
        {
            GraphDataChange = true
        }, cancellationToken);
    }
}