using StrikeforceInfinity.Client.Payloads;

namespace StrikeforceInfinity.Server.Items
{
    public class GameHost
    {
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The sessionid of the connection that created the host.
        /// </summary>
        public Guid OwnerSessionId { get; set; }

        public Guid UID { get; set; }
        public string Name { get; set; }
        public int MaxPlayers { get; set; }

        public GameHost(Guid ownerSessionId, SiGameHost configuration)
        {
            UID = Guid.NewGuid();
            OwnerSessionId = ownerSessionId;
            Name = configuration.Name;
            MaxPlayers = configuration.MaxPlayers;
        }
    }
}
