namespace StrikeforceInfinity.Client.Payloads
{
    public class SiGameHost
    {
        public Guid UID { get; set; }
        public string Name { get; set; } = string.Empty;
        public int MaxPlayers { get; set; } = 100;

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
