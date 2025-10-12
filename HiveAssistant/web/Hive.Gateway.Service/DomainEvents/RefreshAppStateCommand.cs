﻿using Core.App.Handlers;
using Core.Contract;
using Hive.Gateway.Service.Services;

namespace Hive.Gateway.Service.DomainEvents;

public class RefreshAppStateCommand : ICommand
{
    public bool RefreshAppState { get; set; }
    public bool GraphDataChange { get; set; }
}

public sealed class RefreshAppStateCommandhandler(AppState appState) : ICommandAsyncHandler<RefreshAppStateCommand>
{
    public int Order => 0;

    public ValueTask<string> AsyncTaskId(RefreshAppStateCommand cmd)
    {
        return ValueTask.FromResult(nameof(AppState));
    }

    public async Task HandleCommand(RefreshAppStateCommand cmd, CancellationToken cancellationToken)
    {
        if (cmd.RefreshAppState)
            await appState.NotifyTimeSeriesAdded();
        if (cmd.GraphDataChange)
            await appState.NotifyGraphDataChange();
    }
}