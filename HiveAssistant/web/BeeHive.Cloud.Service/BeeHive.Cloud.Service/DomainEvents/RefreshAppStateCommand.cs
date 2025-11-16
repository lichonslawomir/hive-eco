using BeeHive.Cloud.Service.Hubs;
using BeeHive.Cloud.Service.Services;
using Core.App.Handlers;
using Core.Contract;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BeeHive.Cloud.Service.DomainEvents;

public class RefreshAppStateCommand : ICommand
{
    public bool RefreshAppState { get; set; }
    public bool GraphDataChange { get; set; }
}

public sealed class RefreshAppStateCommandhandler(AppState appState,
    IHubContext<RefreshHub> hubContext) : ICommandAsyncHandler<RefreshAppStateCommand>
{
    public int Order => 0;

    public ValueTask<string> AsyncTaskId(RefreshAppStateCommand cmd)
    {
        return ValueTask.FromResult(nameof(AppState));
    }

    public async Task HandleCommand(RefreshAppStateCommand cmd, CancellationToken cancellationToken)
    {
        if (cmd.RefreshAppState)
        {
            await hubContext.Clients.All.NotifyTimeSeriesAdded();
            await appState.NotifyTimeSeriesAdded();
        }
        if (cmd.GraphDataChange)
        {
            await hubContext.Clients.All.NotifyTimeSeriesAdded();
            await appState.NotifyGraphDataChange();
        }
    }
}