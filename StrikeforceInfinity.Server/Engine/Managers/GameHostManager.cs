using NTDLS.Semaphore;
using StrikeforceInfinity.Server.Engine.Objects;
using StrikeforceInfinity.Shared.Payload;

namespace StrikeforceInfinity.Server.Engine.Managers
{
    internal class GameHostManager
    {
        private readonly ServerCore _serverCore;
        readonly PessimisticSemaphore<Dictionary<Guid, GameHost>> _collection = new();

        public GameHostManager(ServerCore serverCore)
        {
            _serverCore = serverCore;
        }

        public GameHost Create(Guid connectionId, SiGameHost configuration)
        {
            return _collection.Use(o =>
            {
                var gameHost = new GameHost(connectionId, configuration.Name, configuration.MaxPlayers);
                {
                };

                o.Add(gameHost.UID, gameHost);
                return gameHost;
            });
        }

        public GameHost? GetByGameHostUID(Guid gameHostUID)
        {
            return _collection.Use(o =>
            {
                o.TryGetValue(gameHostUID, out var gameHost);
                return gameHost;
            });
        }

        public List<SiGameHost> GetList(Guid connectionId)
        {
            return _collection.Use(o =>
            {
                //TODO: What kind of filters should be add??

                var collection = new List<SiGameHost>();

                foreach (var item in o)
                {
                    collection.Add(new SiGameHost()
                    {
                        UID = item.Value.UID,
                        Name = item.Value.Name,
                        MaxPlayers = item.Value.MaxPlayers,
                    });
                }
                return collection;
            });
        }
    }
}
