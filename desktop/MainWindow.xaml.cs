using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.IO.Pipes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using SignalRChat.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading;
using Microsoft.AspNetCore.SignalR;

namespace desktop;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private IHost? signalRHost;
    private SignalRClient? signalRClient;
    private const string SignalRUrl = "http://localhost:5000/chatHub";

    private const string BackendSignalRUrl = "http://localhost:5006/browser-hub";

    // Add this field to your MainWindow class
    private IHubContext<ChatHub> _hubContext;

    public MainWindow()
    {
        InitializeComponent();

        // Subscribe to the ChatHub's MessageSent event
        ChatHub.MessageSent += OnMessageSent;

        StartSignalRHost();

        // StartNamedPipeServer();
        InitializeSignalRClient();
    }

    // Handler for messages received from SignalR clients
    private void OnMessageSent(string user, string message)
    {
        // Since this might be called from a background thread, use Dispatcher
        Dispatcher.Invoke(() =>
        {
            receiveTextBox.Text = $"{user}: {message}";
        });
    }



    private async void InitializeSignalRClient()
    {
        // Wait a bit for the server to start

        signalRClient = new SignalRClient(BackendSignalRUrl);
        signalRClient.MessageReceived += (user, message) =>
        {
            Dispatcher.Invoke(() =>
            {
                receiveTextBox.Text = $"{user}: {message}\n";
            });
        };

        try
        {
            await signalRClient.ConnectAsync();
            // statusTextBox.Text = $"connected to backend signalR {signalRClient}";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to connect to SignalR hub: {ex.Message}");
        }
    }

    private void StartSignalRHost()
    {

        Task.Run(async () =>
    {
        signalRHost = CreateHostBuilder([]).Build();
        await signalRHost.StartAsync();

        // Get the hub context right after starting the host
        var scope = signalRHost.Services.CreateScope();
        _hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<ChatHub>>();

        Dispatcher.Invoke(() =>
        {
            statusTextBox.Text = "SignalR hub host created!";
        });
    });
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseUrls("http://localhost:5000");
            });

    private void StartNamedPipeServer()
    {
        Task.Run(() =>
        {
            while (true)
            {
                using var server = new NamedPipeServerStream("MyPipe", PipeDirection.InOut, 1, PipeTransmissionMode.Message);
                server.WaitForConnection();

                using var sr = new StreamReader(server);
                using var sw = new StreamWriter(server) { AutoFlush = true };

                string? msg = sr.ReadLine();
                if (msg == null) continue;

                // Process and compute response
                var resp = new { status = "ok", echo = msg };
                sw.WriteLine(JObject.FromObject(resp).ToString());

                // Update UI on the main thread
                Dispatcher.Invoke(() =>
                {
                    try
                    {
                        // Parse the incoming JSON
                        JObject jsonMsg = JObject.Parse(msg);

                        // Display the message in the UI
                        if (jsonMsg["message"] != null)
                        {
                            receiveTextBox.Text = jsonMsg["message"]?.ToString();
                        }
                        else
                        {
                            receiveTextBox.Text = jsonMsg.ToString(Formatting.None);
                        }
                    }
                    catch (Exception ex)
                    {
                        // If not valid JSON, just show the raw message
                        receiveTextBox.Text = ex.Message;
                    }
                });
            }
        });
    }

    async void SendInput(object sender, RoutedEventArgs e) // send data to chrome extension
    {
        if (string.IsNullOrEmpty(textInput.Text))
            return;

        try
        {
            // Get the hub context if it's not already set
            if (_hubContext == null)
            {
                var scope = signalRHost!.Services.CreateScope();
                _hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<ChatHub>>();
            }

            // This directly calls the "ReceiveMessage" method on all clients
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "Desktop App", textInput.Text);
            // TODO: this breaches encapsulation
            textInput.Clear();
        }
        catch (Exception ex)
        {
            statusTextBox.Text = ex.Message;
        }

        // if (signalRClient?.IsConnected == true)
        // {
        //     try
        //     {
        //         await signalRClient.SendMessageAsync("WPF App", textInput.Text);
        //         textInput.Clear();
        //     }
        //     catch (Exception ex)
        //     {
        //         MessageBox.Show($"Error sending message: {ex.Message}");
        //     }
        // }
        // else
        // {
        //     Console.WriteLine(textInput.Text);
        // }
    }

    protected override async void OnClosed(EventArgs e)
    {
        // Unsubscribe from events
        ChatHub.MessageSent -= OnMessageSent;

        if (signalRClient != null)
            await signalRClient.DisconnectAsync();

        // Stop SignalR host properly
        if (signalRHost != null)
        {
            await signalRHost.StopAsync();
            await signalRHost.WaitForShutdownAsync(); // Important!
        }

        base.OnClosed(e);
        Application.Current.Shutdown();
    }
}