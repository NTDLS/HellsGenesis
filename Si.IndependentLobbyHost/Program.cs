
using Si.Library.Payload;
using Si.MultiplayClient;

namespace Si.IndependentLobbyHost
{
    /// <summary>
    /// Uses the Si.Multiplay module to allow for hosting lobbies for human players independent from the Si.Game.
    /// </summary>
    internal class Program
    {
        static void Main(string[] args)
        {
            CreateEmptyLobbyies();

            Console.WriteLine("Press [enter] to exit.");
            Console.ReadLine();
        }

        /// <summary>
        /// Lobbies without any attached game engine. These will only have human players.
        /// </summary>
        private static void CreateEmptyLobbyies()
        {
            Console.WriteLine("Starting multiplay...");
            var multiplay = new EngineMultiplayManager();
            Console.WriteLine("Success!");

            //multiplay.OnReceivedSituationLayout += Multiplay_OnReceivedSituationLayout;
            //multiplay.OnNeedSituationLayout += Multiplay_OnNeedSituationLayout;
            //multiplay.OnSpriteVectorsUpdated += Multiplay_OnSpriteVectorsUpdated;

            Console.WriteLine("Configuring server connection...");
            multiplay.ConfigureConnection();
            Console.WriteLine("Success!");

            Console.WriteLine($"Creating lobby...");
            multiplay.CreateLobby(new SiLobbyConfiguration("Boaring Hoast", 1, 10, 30) { IsHeadless = true });
            Console.WriteLine("Success!");

            Console.WriteLine($"Creating lobby...");
            multiplay.CreateLobby(new SiLobbyConfiguration("Werbenjägermanjensen", 2, 10, 30) { IsHeadless = true });
            Console.WriteLine("Success!");

            Console.WriteLine($"Creating lobby...");
            multiplay.CreateLobby(new SiLobbyConfiguration("Deep Space -11", 5, 10, 30) { IsHeadless = true });
            Console.WriteLine("Success!");
        }
    }
}
