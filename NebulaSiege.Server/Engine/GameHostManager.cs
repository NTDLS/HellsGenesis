using NebulaSiege.Client.Payloads;
using NebulaSiege.Server.Items;
using NTDLS.Semaphore;

namespace NebulaSiege.Server.Engine
{
    public class GameHostManager
    {
        private readonly ServerCore _core;
        readonly PessimisticSemaphore<List<GameHost>> _hosts = new();

        public GameHostManager(ServerCore core)
        {
            _core = core;
        }

        public GameHost Create(Guid ownerSessionId, NsGameHost configuration)
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

        public List<NsGameHost> GetList(Guid ownerSessionId, NsGameHostFilter gameHostFilter)
        {
            return _hosts.Use(o =>
            {
                //TODO: What kind of filters should be add??

                var collection = new List<NsGameHost>();

                foreach (var item in o)
                {
                    collection.Add(new NsGameHost()
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
