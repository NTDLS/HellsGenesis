using NTDLS.Semaphore;
using Si.ServerEngine.Core.Objects;
using Si.Shared.Payload;
using System.Diagnostics.CodeAnalysis;

namespace Si.ServerEngine.Core.Managers
{
    internal class LobbyManager
    {
        private readonly ServerEngineCore _serverCore;
        private readonly PessimisticSemaphore<Dictionary<Guid, Lobby>> _collection = new();

        public LobbyManager(ServerEngineCore serverCore)
        {
            _serverCore = serverCore;
        }

        public Lobby Create(Guid connectionId, SiLobbyConfiguration configuration)
        {
            return _collection.Use(o =>
            {
                var lobby = new Lobby(_serverCore, connectionId, configuration);
                {
                    //...
                };

                o.Add(lobby.UID, lobby);
                return lobby;
            });
        }

        public void Delete(Guid lobbyUID)
        {
            _collection.Use(o =>
            {
                if (TryGetByLobbyUID(lobbyUID, out var lobby))
                {
                    lobby.Cleanup();
                }

                o.Remove(lobbyUID);
            });
        }


        public bool TryGetByLobbyOwnerConnectionId(Guid connectionId, [NotNullWhen(true)] out Lobby? outLobby)
        {
            var result = _collection.Use(o =>
            {
                return o.Values.Where(x => x.OwnerConnectionId == connectionId).SingleOrDefault();
            });

            outLobby = result;
            return outLobby != null;
        }

        public bool TryGetByLobbyUID(Guid lobbyUID, [NotNullWhen(true)] out Lobby? outLobby)
        {
            var result = _collection.Use(o =>
            {
                o.TryGetValue(lobbyUID, out var lobby);
                return lobby;
            });

            outLobby = result;
            return outLobby != null;
        }

        public List<SiLobbyConfiguration> GetList()
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
