using System.Net.Sockets;
using System.Text;

namespace Client
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var masterServerIp = "127.0.0.1";
            var masterServerPort = 5000;

            using var client = new TcpClient();
            client.Connect(masterServerIp, masterServerPort);

            var message = "GET_SERVER Алмаз";
            var stream = client.GetStream();
            var buffer = Encoding.UTF8.GetBytes(message);
            stream.Write(buffer, 0, buffer.Length);

            var responseBuffer = new byte[1024];
            int bytesRead = stream.Read(responseBuffer, 0, responseBuffer.Length);
            string response = Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);

            Console.WriteLine($"Server Info: {response}");

            var serverInfo = response.Split(':');
            var gameServerIp = serverInfo[0];
            var gameServerPort = int.Parse(serverInfo[1]);

            using var gameClient = new TcpClient();
            gameClient.Connect(gameServerIp, gameServerPort);

            var gameStream = gameClient.GetStream();
            var gameMessage = Encoding.UTF8.GetBytes("Hello Game Server!");
            gameStream.Write(gameMessage, 0, gameMessage.Length);

            bytesRead = gameStream.Read(responseBuffer, 0, responseBuffer.Length);
            string gameResponse = Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);

            Console.WriteLine($"Game Server Response: {gameResponse}");
        }
    }
