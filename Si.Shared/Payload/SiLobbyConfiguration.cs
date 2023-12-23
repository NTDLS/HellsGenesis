namespace Si.Shared.Payload
{
    public class SiLobbyConfiguration
    {
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public Guid OwnerSessionId { get; set; }
        public Guid UID { get; set; }

        public string Name { get; set; } = string.Empty;
        public int MinPlayers { get; set; }
        public int MaxPlayers { get; set; }
        public int AutoStartSeconds { get; set; }

        public SiLobbyConfiguration(string lobbyName, int minPlayers, int maxPlayers, int autoStartSeconds)
        {
            Name = lobbyName;
            MinPlayers = minPlayers;
            MaxPlayers = maxPlayers;
            AutoStartSeconds = autoStartSeconds;
        }

        public SiLobbyConfiguration()
        {
        }
    }
}
