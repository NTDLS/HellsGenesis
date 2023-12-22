namespace StrikeforceInfinity.Shared.Payload
{
    public class SiLobbyInfo
    {
        public string Name { get; set; } = string.Empty;
        public int WaitingCount { get; set; }
        public List<SiLobbyConnection> Connections { get; set; } = new();
    }
}
