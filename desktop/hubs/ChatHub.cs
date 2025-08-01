using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {

        // Static event that the MainWindow can subscribe to
        public static event Action<string, string> MessageSent;
        public static event Action<string, string> MessageReceived;


        public override async Task OnConnectedAsync()
        {
            // Log connection ID when a client connects
            string clientId = Context.ConnectionId;
            // Console.WriteLine($"Client connected: {clientId}");

            // Send acknowledgment to the specific client that just connected
            await Clients.Caller.SendAsync("ConnectionAcknowledged", "Desktop app acknowledges signalR connection");

            await base.OnConnectedAsync();
        }
        public async Task SendMessage(string user, string message)
        {
            // await Clients.All.SendAsync("ReceiveMessage", user, message);

            // Trigger the static event so MainWindow can update the UI
            MessageSent?.Invoke(user, message);

            // Send acknowledgment back to the caller
            await Clients.Caller.SendAsync("MessageAcknowledged");
        }

        public async Task ReceiveMessage(string user, string message)
        {
            // await Clients.All.SendAsync("ReceiveMessage", user, message);

            // await Clients.Caller.SendAsync("ReceiveMessage", "Desktop app acknowledges signalR connection");


            // Send acknowledgment back to the caller
            await Clients.Caller.SendAsync("MessageAcknowledged");
        }
    }
}