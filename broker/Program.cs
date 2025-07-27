using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Threading;

class Broker
{
    static void Main(string[] args)
    {
        // Send "hello world" message to WPF app on startup
        Console.WriteLine("Sending hello world message to WPF app...");
        string helloMessage = "{\"message\":\"hello world\"}";
        
        // Try to connect to WPF app with retry logic
        string response = "";
        bool connected = false;
        int retryCount = 0;
        const int maxRetries = 5;
        
        while (!connected && retryCount < maxRetries)
        {
            try
            {
                response = SendToWpf(helloMessage);
                connected = true;
                Console.WriteLine($"Response from WPF: {response}");
            }
            catch (Exception ex)
            {
                retryCount++;
                Console.WriteLine($"Attempt {retryCount}/{maxRetries}: Could not connect to WPF app. Is it running? ({ex.Message})");
                if (retryCount < maxRetries)
                {
                    Console.WriteLine("Retrying in 2 seconds...");
                    Thread.Sleep(2000);
                }
                else
                {
                    Console.WriteLine("Could not connect to WPF app after multiple attempts. Please make sure it's running.");
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                    return;
                }
            }
        }
        
        var stdin = Console.OpenStandardInput();
        var stdout = Console.OpenStandardOutput();

        while (true)
        {
            // Read length prefix
            byte[] lenBuf = new byte[4];
            if (stdin.Read(lenBuf, 0, 4) != 4) break;
            int len = BitConverter.ToInt32(lenBuf, 0);

            // Read JSON payload
            byte[] msgBuf = new byte[len];
            stdin.Read(msgBuf, 0, len);
            string inputJson = Encoding.UTF8.GetString(msgBuf);

            // Forward to WPF via named pipe
            try
            {
                string replyJson = SendToWpf(inputJson);
                
                // Send back to Chrome
                byte[] replyBytes = Encoding.UTF8.GetBytes(replyJson);
                stdout.Write(BitConverter.GetBytes(replyBytes.Length));
                stdout.Write(replyBytes);
                stdout.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error communicating with WPF app: {ex.Message}");
            }
        }
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
