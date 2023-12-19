namespace StrikeforceInfinity.Shared.Payload
{
    public class SiGameHost
    {
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public Guid OwnerSessionId { get; set; }
        public Guid UID { get; set; }
        public string Name { get; set; } = string.Empty;
        public int MaxPlayers { get; set; }

        public SiGameHost(string name, int maxPlayers)
        {
            Name = name;
            MaxPlayers = maxPlayers;
        }

        public SiGameHost()
        {

        }
    }
}
