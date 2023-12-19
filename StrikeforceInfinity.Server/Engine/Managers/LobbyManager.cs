using NTDLS.Semaphore;
using StrikeforceInfinity.Server.Engine.Objects;
using StrikeforceInfinity.Shared.Payload;

namespace StrikeforceInfinity.Server.Engine.Managers
{
    internal class LobbyManager
    {
        private readonly ServerCore _serverCore;
        readonly PessimisticSemaphore<Dictionary<Guid, Lobby>> _collection = new();

        public LobbyManager(ServerCore serverCore)
        {
            _serverCore = serverCore;
        }

        public Lobby Create(Guid connectionId, SiLobbyConfiguration configuration)
        {
            return _collection.Use(o =>
            {
                var lobby = new Lobby(connectionId, configuration.Name, configuration.MaxPlayers);
                {
                };

                o.Add(lobby.UID, lobby);
                return lobby;
            });
        }

        public Lobby? GetByLobbyUID(Guid lobbyUID)
        {
            return _collection.Use(o =>
            {
                o.TryGetValue(lobbyUID, out var lobby);
                return lobby;
            });
        }

        public List<SiLobbyConfiguration> GetList(Guid connectionId)
        {
            return _collection.Use(o =>
            {
                //TODO: What kind of filters should be add??

                var collection = new List<SiLobbyConfiguration>();

                foreach (var item in o)
                {
                    collection.Add(new SiLobbyConfiguration()
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
