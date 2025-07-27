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


namespace desktop;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        StartNamedPipeServer();
    }

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
                            receiveTextBox.Text = jsonMsg["message"].ToString();
                        }
                        else
                        {
                            receiveTextBox.Text = jsonMsg.ToString(Formatting.None);
                        }
                    }
                    catch (Exception ex)
                    {
                        // If not valid JSON, just show the raw message
                        receiveTextBox.Text = msg;
                    }
                });
            }
        });
    }

    void SendInput(object sender, RoutedEventArgs e)
    {
        Console.WriteLine(textInput.Text);
    }
}