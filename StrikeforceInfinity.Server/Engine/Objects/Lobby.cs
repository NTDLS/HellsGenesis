using NTDLS.Semaphore;

namespace StrikeforceInfinity.Server.Engine.Objects
{
    internal class Lobby
    {
        private readonly PessimisticSemaphore<Dictionary<Guid, RegisteredClientState>> _registeredClients = new();

        /// <summary>
        /// The date and time that the lobby was created.
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The sessionid of the connection that created the lobby.
        /// </summary>
        public Guid OwnerConnectionId { get; set; }

        /// <summary>
        /// The unique id of the  lobby. This is server generated.
        /// </summary>
        public Guid UID { get; set; }

        /// <summary>
        /// User suppled name of the lobby.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The maximum number of players that can register for the lobby.
        /// </summary>
        public int MaxPlayers { get; set; }

        public Lobby(Guid ownerConnectionId, string name, int maxPlayers)
        {
            UID = Guid.NewGuid();
            OwnerConnectionId = ownerConnectionId;
            Name = name;
            MaxPlayers = maxPlayers;
        }

        /// <summary>
        /// Registers a connection for this lobby.
        /// </summary>
        /// <param name="connectionId"></param>
        public void Register(Guid connectionId)
        {
            _registeredClients.Use(o =>
            {
                var state = new RegisteredClientState()
                {
                    ConnectionId = connectionId
                };

                o.Remove(connectionId);
                o.Add(connectionId, state);
            });
        }

        /// <summary>
        /// Removes a connection from a lobby.
        /// </summary>
        /// <param name="connectionId"></param>
        public void Deregister(Guid connectionId)
        {
            _registeredClients.Use(o =>
            {
                o.Remove(connectionId);
            });
        }

        /// <summary>
        /// Tells the server that the connections is ready to start the game.
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns>Returns true if all registered connections are ready to start.</returns>
        public bool FlagConnectionAsReady(Guid connectionId)
        {
            return _registeredClients.Use(o =>
            {
                if (o.TryGetValue(connectionId, out var state))
                {
                    state.IsReadyToPlay = true;
                }

                return o.Values.All(o => o.IsReadyToPlay == true);
            });
        }

        /// <summary>
        /// Returns true if all registered connections are ready to start.
        /// </summary>
        /// <param name="connectionId"></param>
        public bool AreAllConnectionsReady()
        {
            return _registeredClients.Use(o =>
            {
                return o.Values.All(o => o.IsReadyToPlay == true);
            });
        }
    }
}
