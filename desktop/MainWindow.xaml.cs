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

    public MainWindow()
    {
        InitializeComponent();
        StartSignalRHost();

        // StartNamedPipeServer();
        // InitializeSignalRClient();
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
            // MessageBox.Show("Connected to SignalR hub!");
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

            Dispatcher.Invoke(() =>
            {
                statusTextBox.Text = "SignalR hub host created!";
            });

            // MessageBox.Show("SignalR hub host created!");
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

    async void SendInput(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(textInput.Text))
            return;

        if (signalRClient?.IsConnected == true)
        {
            try
            {
                await signalRClient.SendMessageAsync("WPF App", textInput.Text);
                textInput.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending message: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine(textInput.Text);
        }
    }

    protected override async void OnClosed(EventArgs e)
    {
        if (signalRClient != null)
            await signalRClient.DisconnectAsync();

        signalRHost?.StopAsync().Wait();
        base.OnClosed(e);
    }
}