using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace desktop
{
    public class SignalRClient
    {
        private readonly HubConnection connection;
        public event Action<string, string>? MessageReceived;

        public SignalRClient(string url)
        {
            connection = new HubConnectionBuilder()
                .WithUrl(url)
                .WithAutomaticReconnect()
                .Build();

            connection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                MessageReceived?.Invoke(user, message);
            });
        }

        public async Task ConnectAsync()
        {
            await connection.StartAsync();
        }

        public async Task SendMessageAsync(string user, string message)
        {
            await connection.InvokeAsync("SendMessage", user, message);
        }

        public async Task DisconnectAsync()
        {
            await connection.StopAsync();
        }

        public bool IsConnected => connection.State == HubConnectionState.Connected;
    }
} 