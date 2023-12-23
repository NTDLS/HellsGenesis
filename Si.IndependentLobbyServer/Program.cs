using Si.Multiplay;

namespace Si.IndependentLobbyServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var multiplay = new EngineMultiplayManager();
            //Multiplay.OnReceivedSituationLayout += Sprites.ApplySituationLayout;
            //Multiplay.OnNeedSituationLayout += Sprites.GetSituationLayout;
            //Multiplay.OnSpriteVectorsUpdated += Sprites.UpdateSpriteVectors;


            multiplay.ConfigureConnection();

            multiplay.CreateLobby(new Shared.Payload.SiLobbyConfiguration("Boaring Hoast", 1, 10, 30));
            multiplay.CreateLobby(new Shared.Payload.SiLobbyConfiguration("Werbenjägermanjensen", 2, 10, 30));
            multiplay.CreateLobby(new Shared.Payload.SiLobbyConfiguration("Deep Space -11", 5, 10, 30));

            Console.ReadLine();


        }
    }
}
