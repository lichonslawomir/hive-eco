using Microsoft.AspNetCore.SignalR;

namespace BeeHive.Cloud.Service.Hubs;

public class RefreshHub(ILogger<RefreshHub> logger) : Hub
{
    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }

    public void StartListening()
    {
        logger.LogInformation("StartListening: {ConnectionId}", Context.ConnectionId);
    }

    public void StopListening()
    {
        logger.LogInformation("StopListening: {ConnectionId}", Context.ConnectionId);
    }

    public async Task NotifyGraphDataChange()
    {
        await Clients.All.NotifyGraphDataChange();
    }

    public async Task NotifyTimeSeriesAdded()
    {
        await Clients.All.NotifyTimeSeriesAdded();
    }
}

public static class RefreshHubExtensions
{
    public static async Task NotifyGraphDataChange(this IClientProxy clients)
    {
        await clients.SendAsync("GraphDataChanged");
    }

    public static async Task NotifyTimeSeriesAdded(this IClientProxy clients)
    {
        await clients.SendAsync("TimeSeriesAdded");
    }
}