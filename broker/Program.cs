using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;

class Broker
{
    static void Main(string[] args)
    {
        // // Set up error logging to a file for debugging
        // string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "broker_log.txt");
        // using var logFile = new StreamWriter(logPath, true) { AutoFlush = true };
        // Console.SetError(logFile);
        // Console.Error.WriteLine($"Broker started at {DateTime.Now}");

        // // Try to connect to WPF app with retry logic
        // bool wpfConnected = TryConnectToWpf(logFile);
        // if (!wpfConnected && !Console.IsInputRedirected)
        // {
        //     // If running standalone (not from Chrome) and WPF isn't available, exit
        //     Console.WriteLine("Press any key to exit...");
        //     Console.ReadKey();
        //     return;
        // }

        // Begin Chrome native messaging protocol
        var stdin = Console.OpenStandardInput();
        var stdout = Console.OpenStandardOutput();

        try
        {
            // Send "hello world" message to Chrome extension immediately on startup
            SendHelloWorldToChrome(stdout);

            while (true)
            {
                // Read length prefix (4 bytes) from Chrome
                byte[] lenBuf = new byte[4];
                int bytesRead = stdin.Read(lenBuf, 0, 4);

                if (bytesRead != 4)
                {
                    Console.Error.WriteLine("Failed to read message length. Extension may have closed the connection.");
                    break;
                }

                int len = BitConverter.ToInt32(lenBuf, 0);
                if (len <= 0)
                {
                    Console.Error.WriteLine($"Invalid message length: {len}");
                    break;
                }

                // Read JSON payload
                byte[] msgBuf = new byte[len];
                stdin.Read(msgBuf, 0, len);
                string inputJson = Encoding.UTF8.GetString(msgBuf);
                Console.Error.WriteLine($"Received from Chrome: {inputJson}");

                // Process the message
                JObject inputObj = JObject.Parse(inputJson);
                string responseJson;

                responseJson = JsonConvert.SerializeObject(new
                {
                    reply = $"Received: {inputObj["text"]?.ToString() ?? "no text"}",
                    timestamp = DateTime.Now.ToString()
                });

                // // Forward to WPF if connected
                // if (wpfConnected)
                // {
                //     try
                //     {
                //         responseJson = SendToWpf(inputJson);
                //     }
                //     catch (Exception ex)
                //     {
                //         Console.Error.WriteLine($"Error communicating with WPF app: {ex.Message}");
                //         responseJson = JsonConvert.SerializeObject(new { error = $"WPF communication error: {ex.Message}" });
                //     }
                // }
                // else
                // {
                //     // If WPF isn't connected, respond directly
                //     responseJson = JsonConvert.SerializeObject(new { 
                //         reply = $"Received: {inputObj["text"]?.ToString() ?? "no text"}", 
                //         timestamp = DateTime.Now.ToString() 
                //     });
                // }

                // Send response back to Chrome (length prefix + JSON)
                SendMessageToChrome(stdout, responseJson);
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Fatal error: {ex}");
        }
    }

    static void SendHelloWorldToChrome(Stream stdout)
    {
        try
        {
            string helloJson = JsonConvert.SerializeObject(new
            {
                message = "hello world",
                source = "broker",
                timestamp = DateTime.Now.ToString()
            });

            SendMessageToChrome(stdout, helloJson);
            Console.Error.WriteLine($"Sent hello world message to Chrome: {helloJson}");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to send hello world message: {ex.Message}");
        }
    }

    static void SendMessageToChrome(Stream stdout, string json)
    {
        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
        stdout.Write(BitConverter.GetBytes(jsonBytes.Length), 0, 4);
        stdout.Write(jsonBytes, 0, jsonBytes.Length);
        stdout.Flush();
    }

    static bool TryConnectToWpf(TextWriter log)
    {
        // Send "hello world" message to WPF app on startup
        log.WriteLine("Attempting to connect to WPF app...");
        string helloMessage = "{\"message\":\"hello world\"}";

        bool connected = false;
        int retryCount = 0;
        const int maxRetries = 5;

        while (!connected && retryCount < maxRetries)
        {
            try
            {
                string response = SendToWpf(helloMessage);
                connected = true;
                log.WriteLine($"WPF connection successful. Response: {response}");
                Console.WriteLine("Connected to WPF application.");
            }
            catch (Exception ex)
            {
                retryCount++;
                log.WriteLine($"Attempt {retryCount}/{maxRetries}: Could not connect to WPF app. ({ex.Message})");
                if (retryCount < maxRetries)
                {
                    log.WriteLine("Retrying in 2 seconds...");
                    Thread.Sleep(2000);
                }
                else
                {
                    log.WriteLine("Could not connect to WPF app after multiple attempts.");
                    Console.WriteLine("WPF application not available. Running in standalone mode.");
                }
            }
        }

        return connected;
    }

    static string SendToWpf(string json)
    {
        using var client = new NamedPipeClientStream(".", "MyPipe", PipeDirection.InOut);
        client.Connect(2000); // 2 second timeout
        using var sw = new StreamWriter(client) { AutoFlush = true };
        using var sr = new StreamReader(client);
        sw.WriteLine(json);
        return sr.ReadLine() ?? "{}";
    }
}
