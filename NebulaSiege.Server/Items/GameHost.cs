using NebulaSiege.Client.Payloads;

namespace NebulaSiege.Server.Items
{
    public class GameHost
    {
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The sessionif of the connection that created the host.
        /// </summary>
        public Guid OwnerSessionId { get; set; }

        public string Name { get; set; }
        public int MaxPlayers { get; set; }

        public GameHost(Guid ownerSessionId, NsGameHost configuration)
        {
            OwnerSessionId = ownerSessionId;
            Name = configuration.Name;
            MaxPlayers = configuration.MaxPlayers;
        }
    }
}
