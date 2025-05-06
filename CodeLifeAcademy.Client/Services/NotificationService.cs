using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace CodeLifeAcademy.Client.Services;

public class NotificationService
{
    private readonly NavigationManager _navigationManager;
    private HubConnection? _hubConnection;

    public event Action<string, string>? OnMessageReceived;
    public event Action? OnDataRefreshRequested;

    public NotificationService(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
    }

    private bool _handlersRegistered = false;

    public async Task ConnectAsync()
    {
        if (_hubConnection is { State: HubConnectionState.Connected})
        {
            return;
        }

        if (_hubConnection == null)
        {
            var hubUrl = new Uri("https://localhost:7271/hubs/notification");

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .WithAutomaticReconnect()
                .Build();
        }

        if (!_handlersRegistered)
        {
            _hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                OnMessageReceived?.Invoke(user, message);
            });

            _hubConnection.On("DataRefreshRequested", () =>
            {
                Console.WriteLine("Получен сигнал DataRefreshRequested от сервера");
                OnDataRefreshRequested?.Invoke();
            });

            _handlersRegistered = true;
        }

        if (_hubConnection.State == HubConnectionState.Disconnected)
        {
            await _hubConnection.StartAsync();
        }
    }

    public async Task SendMessageAsync(string user, string message)
    {
        if (_hubConnection is not null && _hubConnection.State == HubConnectionState.Connected)
        {
            await _hubConnection.SendAsync("SendMessage", user, message);
        }
    }

    public async Task RequestDataRefresh()
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            await _hubConnection.SendAsync("BroadcastDataRefresh");
        }
    }
}
