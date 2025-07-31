using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace SignalRConsoleClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Create a connection to the SignalR hub
            // Replace with your actual hub URL
            var hubConnection = new HubConnection("http://localhost:5000/");
            
            // Enable client-side logging for debugging
            hubConnection.TraceLevel = TraceLevels.All;
            hubConnection.TraceWriter = Console.Out;
            
            // Create a proxy to the ChatHub
            IHubProxy chatHubProxy = hubConnection.CreateHubProxy("ChatHub");
            
            // Register for method calls from the server
            chatHubProxy.On<string, string>("ReceiveMessage", (user, message) => 
                Console.WriteLine($"{user}: {message}")
            );
            
            // Handle connection lifecycle events
            hubConnection.Reconnecting += () => Console.WriteLine("Reconnecting...");
            hubConnection.Reconnected += () => Console.WriteLine("Reconnected");
            hubConnection.ConnectionSlow += () => Console.WriteLine("Connection is slow");
            hubConnection.Closed += () => Console.WriteLine("Connection closed");
            hubConnection.Error += ex => Console.WriteLine($"Error: {ex.Message}");
            
            try
            {
                // Start the connection
                await hubConnection.Start();
                Console.WriteLine("Connected to SignalR hub. Enter messages or type 'exit' to quit.");
                
                // Keep the application running to receive messages
                string username = "ConsoleUser";
                Console.Write("Enter your name: ");
                string input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    username = input;
                }
                
                // Main message loop
                while (true)
                {
                    Console.Write("Enter message (or 'exit' to quit): ");
                    string message = Console.ReadLine();
                    
                    if (string.IsNullOrEmpty(message) || message.ToLower() == "exit")
                        break;
                    
                    // Call the SendMessage method on the hub
                    await chatHubProxy.Invoke("SendMessage", username, message);
                }
                
                // Properly dispose the connection
                hubConnection.Stop();
                hubConnection.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to hub: {ex.Message}");
            }
        }
    }
}
