using Microsoft.AspNetCore.SignalR;

namespace CodeLifeAcademy.API.Hubs;

public class NotificationHub: Hub
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    public async Task BroadcastDataRefresh()
    {
        await Clients.All.SendAsync("DataRefreshRequested");
    }
}
