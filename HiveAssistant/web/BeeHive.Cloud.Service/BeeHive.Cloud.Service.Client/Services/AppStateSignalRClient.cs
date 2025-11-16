using BeeHive.Contract.Hives.Models;
using BeeHive.Contract.Interfaces;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;

namespace BeeHive.Cloud.Service.Client.Services;

public class AppStateSignalRClient : IAppState, IAsyncDisposable
{
    private const string urlConst = "refresh-hub";

    private readonly HubConnection _hubConnection;

    public event Func<IList<HiveDto>, Task>? OnHiveCollectionChange;

    public event Func<Task>? OnGraphDataChange;

    public event Func<Task>? OnTimeSeriesAdded;

    public AppStateSignalRClient(IWebAssemblyHostEnvironment environment)
    {
        var baseUrl = environment.BaseAddress;
        if(!baseUrl.EndsWith("/"))
            baseUrl = $"{baseUrl}/";
        _hubConnection = new HubConnectionBuilder()
            .WithUrl($"{baseUrl}{urlConst}")
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<IList<HiveDto>>("HiveCollectionChanged", async (hives) =>
        {
            if (OnHiveCollectionChange != null)
                await OnHiveCollectionChange.Invoke(hives);
        });

        _hubConnection.On("GraphDataChanged", async () =>
        {
            if (OnGraphDataChange != null)
                await OnGraphDataChange.Invoke();
        });

        _hubConnection.On("TimeSeriesAdded", async () =>
        {
            if (OnTimeSeriesAdded != null)
                await OnTimeSeriesAdded.Invoke();
        });
    }

    public async ValueTask StartListeningAsync()
    {
        if (_hubConnection.State != HubConnectionState.Disconnected)
            return;
        await _hubConnection.StartAsync();
        await _hubConnection.InvokeAsync("StartListening");
    }

    public async ValueTask DisposeAsync()
    {
        await _hubConnection.DisposeAsync();
    }
}