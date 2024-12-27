
namespace MasterServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var masterServer = new MasterServer("127.0.0.1", 5000);
            masterServer.Start();
        }
    }
}
