namespace StrikeforceInfinity.Server.Engine.Objects
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

        public GameHost(Guid ownerSessionId, string name, int maxPlayers)
        {
            UID = Guid.NewGuid();
            OwnerSessionId = ownerSessionId;
            Name = name;
            MaxPlayers = maxPlayers;
        }
    }
}
