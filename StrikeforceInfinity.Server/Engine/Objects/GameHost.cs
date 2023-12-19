using NTDLS.Semaphore;

namespace StrikeforceInfinity.Server.Engine.Objects
{
    internal class GameHost
    {
        private readonly PessimisticSemaphore<Dictionary<Guid, RegisteredClientState>> _registeredClients = new();

        /// <summary>
        /// The date and time that the game host was created.
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The sessionid of the connection that created the host.
        /// </summary>
        public Guid OwnerSessionId { get; set; }

        /// <summary>
        /// The unique id of the game host. This is server generated.
        /// </summary>
        public Guid UID { get; set; }

        /// <summary>
        /// User suppled name of the game host.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The maximum number of players that can register for the game host.
        /// </summary>
        public int MaxPlayers { get; set; }

        public GameHost(Guid ownerSessionId, string name, int maxPlayers)
        {
            UID = Guid.NewGuid();
            OwnerSessionId = ownerSessionId;
            Name = name;
            MaxPlayers = maxPlayers;
        }

        /// <summary>
        /// Registers a client for this game host.
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
        /// Removes a connection from a game host.
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
        /// Tells the server that the client is ready to start the game.
        /// </summary>
        /// <param name="connectionId"></param>
        public void FlagAsReady(Guid connectionId)
        {
            _registeredClients.Use(o =>
            {
                if (o.TryGetValue(connectionId, out var state))
                {
                    state.IsReadyToPlay = true;
                }
            });
        }
    }
}
