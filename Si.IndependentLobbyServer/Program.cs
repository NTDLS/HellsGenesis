using Si.Multiplay;

namespace Si.IndependentLobbyServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var multiplay = new EngineMultiplayManager();
            //multiplay.OnReceivedSituationLayout += Multiplay_OnReceivedSituationLayout;
            //multiplay.OnNeedSituationLayout += Multiplay_OnNeedSituationLayout;
            //multiplay.OnSpriteVectorsUpdated += Multiplay_OnSpriteVectorsUpdated;

            multiplay.ConfigureConnection();

            multiplay.CreateLobby(new Shared.Payload.SiLobbyConfiguration("Boaring Hoast", 1, 10, 30));
            multiplay.CreateLobby(new Shared.Payload.SiLobbyConfiguration("Werbenjägermanjensen", 2, 10, 30));
            multiplay.CreateLobby(new Shared.Payload.SiLobbyConfiguration("Deep Space -11", 5, 10, 30));

            Console.ReadLine();
        }
    }
}
