namespace NebulaSiege.Client.Payloads
{
    public class NsGameHost
    {
        public Guid UID { get; set; }
        public string Name { get; set; } = string.Empty;
        public int MaxPlayers { get; set; } = 100;

        public NsGameHost(string name, int maxPlayers)
        {
            Name = name;
            MaxPlayers = maxPlayers;
        }

        public NsGameHost()
        {
        }
    }
}
