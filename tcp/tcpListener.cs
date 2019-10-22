using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text;

class MyTcpListener
{
    public async Task StartServerAsync()
    {
        TcpListener server = null;
        try
        {
            // Set the TcpListener on port 13000.
            Int32 port = 5000;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");

            server = new TcpListener(localAddr, port);

            // Start listening for client requests.
            server.Start();

            // Enter the listening loop.
            while (true)
            {
                Console.WriteLine("Waiting for a connection... ");
                var client = await server.AcceptTcpClientAsync();
                Accept(client);
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }
        finally
        {
            // Stop listening for new clients.
            server.Stop();
        }


        Console.WriteLine("\nHit enter to continue...");
        Console.Read();
    }

    public async Task Accept(TcpClient client)
    {
        // Buffer for reading data
        Byte[] bytes = new Byte[5000];
        String data = null;
        var catDB = new CategoryDB();

        await Task.Yield();
        try
        {
            using (client)
            using (NetworkStream n = client.GetStream())
            {
                int i;
                // Loop to receive all the data sent by the client.
                while ((i = n.Read(bytes, 0, bytes.Length)) != 0)
                {
                    // Translate data bytes to a ASCII string.
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                    var request = JsonSerializer.Deserialize<JsonRequest>(data);
                    Console.WriteLine("Received: {0}", data);

                    // Process the data sent by the client.
                    var response = catDB.delegater(request);

                    // Send back a response.
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(response);
                    await n.WriteAsync(msg, 0, msg.Length);
                    await n.FlushAsync();
                    Console.WriteLine("Sent: {0}", response);
                }

                // Shutdown and end connection
                client.Close();
            }
        }
        catch (Exception ex) { Console.WriteLine(ex.Message); }
    }
}