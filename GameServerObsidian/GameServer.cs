using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServerObsidian
{
    class GameServer
    {
        private TcpListener _listener;
        private string _masterServerIp;
        private int _masterServerPort;
        private string _serverName;
        private int _port;

        public GameServer(string serverName, string masterServerIp, int masterServerPort, int port)
        {
            _serverName = serverName;
            _masterServerIp = masterServerIp;
            _masterServerPort = masterServerPort;
            _port = port;

            _listener = new TcpListener(IPAddress.Any, port);
        }


        public void Start()
        {
            RegisterToMasterServer();

            _listener.Start();
            Console.WriteLine($"{_serverName} started on port {_port}...");

            while (true)
            {
                var client = _listener.AcceptTcpClient();
                var clientTask = new Task(() => HandleClient(client));
                clientTask.Start();
            }
        }

        private void RegisterToMasterServer()
        {
            using var client = new TcpClient();
            client.Connect(_masterServerIp, _masterServerPort);

            var message = $"REGISTER_SERVER {_serverName} {GetLocalIPAddress()} {_port}";
            var stream = client.GetStream();
            var buffer = Encoding.UTF8.GetBytes(message);
            stream.Write(buffer, 0, buffer.Length);

            var responseBuffer = new byte[1024];
            int bytesRead = stream.Read(responseBuffer, 0, responseBuffer.Length);
            string response = Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);

            Console.WriteLine($"Master Server Response: {response}");
        }

        private void HandleClient(TcpClient client)
        {
            var stream = client.GetStream();
            var buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            Console.WriteLine($"Client message: {message}");

            var response = Encoding.UTF8.GetBytes("Welcome to the game server!");
            stream.Write(response, 0, response.Length);

            client.Close();
        }


        private string GetLocalIPAddress()
        {
            foreach (var ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }
    }
}
