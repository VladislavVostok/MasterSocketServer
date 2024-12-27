namespace GameServerObsidian
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var gameServer = new GameServer("Алмаз", "127.0.0.1", 5000, 6000);
            gameServer.Start();
        }
    }
}
