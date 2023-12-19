using NTDLS.Semaphore;

namespace StrikeforceInfinity.Server.Engine.Objects
{
    internal class GameHost
    {
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



        PessimisticSemaphore<Dictionary<Guid, RegisteredClientState>> RegisteredClients = new();

        public GameHost(Guid ownerSessionId, string name, int maxPlayers)
        {
            UID = Guid.NewGuid();
            OwnerSessionId = ownerSessionId;
            Name = name;
            MaxPlayers = maxPlayers;
        }
    }
}
