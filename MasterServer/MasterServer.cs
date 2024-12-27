using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MasterServer
{
    class MasterServer
    {
        private TcpListener _listener;
        private ConcurrentDictionary<string, IPEndPoint> _gameServers = new ConcurrentDictionary<string, IPEndPoint>();

        public MasterServer(string host, int port)
        {
            _listener = new TcpListener(IPAddress.Parse(host), port);
        }

        public void Start() { 
        
            _listener.Start();
            Console.WriteLine("Master Server started...");
            while (true) { 
                var client = _listener.AcceptTcpClient();
                var clientTask = new Task(() =>  HandleClient(client));
                clientTask.Start();
            }
        }


        private void HandleClient(TcpClient client) { 
            var stream = client.GetStream(); 
            var buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string message = Encoding.UTF8.GetString(buffer);

            Console.WriteLine($"Received: {message}");

            if (message.StartsWith("REGISTER_SERVER"))
            {
                var parts = message.Split(' ');
                string serverName = parts[1];
                string serverIp = parts[2];
                int serverPort = int.Parse(parts[3]);

                _gameServers[serverName] = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);
                Console.WriteLine($"Registered server: {serverName} ({serverIp}:{serverPort})");

                var response = Encoding.UTF8.GetBytes("OK");
                stream.Write(response, 0, response.Length);
            }
            else if (message.StartsWith("GET_SERVER"))
            {
                var parts = message.Split(' ');
                string serverName = parts[1];

                if (_gameServers.TryGetValue(serverName, out var serverEndpoint))
                {
                    var response = Encoding.UTF8.GetBytes($"{serverEndpoint.Address}:{serverEndpoint.Port}");
                    stream.Write(response, 0, response.Length);
                }

                else
                {
                    var response = Encoding.UTF8.GetBytes("SERVER_NOT_FOUND");
                    stream.Write(response, 0, response.Length);
                }

                client.Close();
            }

        }
    }
}
