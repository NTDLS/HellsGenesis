using NTDLS.Semaphore;
using StrikeforceInfinity.Client.Payloads;
using StrikeforceInfinity.Server.Items;

namespace StrikeforceInfinity.Server.Engine
{
    public class GameHostManager
    {
        private readonly ServerCore _core;
        readonly PessimisticSemaphore<List<GameHost>> _hosts = new();

        public GameHostManager(ServerCore core)
        {
            _core = core;
        }

        public GameHost Create(Guid ownerSessionId, SiGameHost configuration)
        {
            return _hosts.Use(o =>
            {
                var gameHost = new GameHost(ownerSessionId, configuration);
                {
                };

                o.Add(gameHost);
                return gameHost;
            });
        }

        public List<SiGameHost> GetList(Guid ownerSessionId, SiGameHostFilter gameHostFilter)
        {
            return _hosts.Use(o =>
            {
                //TODO: What kind of filters should be add??

                var collection = new List<SiGameHost>();

                foreach (var item in o)
                {
                    collection.Add(new SiGameHost()
                    {
                        UID = item.UID,
                        Name = item.Name,
                        MaxPlayers = item.MaxPlayers,
                    });
                }
                return collection;
            });
        }
    }
}
