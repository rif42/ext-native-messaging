using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using System.Windows;
using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;


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

            MessageBox.Show($"Connected to SignalR hub! {connection.ConnectionId?.ToString()}");
            MessageBox.Show($"Response: {await SendPostRequest(connection.ConnectionId!)}");

        }

        public async Task SendMessageAsync(string user, string message)
        {
            await connection.InvokeAsync("SendMessage", user, message);
        }

        public async Task DisconnectAsync()
        {
            await connection.StopAsync();
        }

        public async Task<string> SendPostRequest(string connectionId)
        {
            using (var client = new HttpClient())
            {
                string url = "http://localhost:5006/api/execute-goal";

                var data = new
                {
                    UserGoal = "search cat pictures", 
                    ConnectionId = connectionId
                };

                string jsonContent = JsonConvert.SerializeObject(data);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                try
                {
                    HttpResponseMessage response = await client.PostAsync(url, content);
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException ex)
                {
                    return $"Error: {ex.Message}";
                }
            }
        }

        public bool IsConnected => connection.State == HubConnectionState.Connected;
    }
}