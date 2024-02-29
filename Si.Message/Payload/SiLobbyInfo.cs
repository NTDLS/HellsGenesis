namespace Si.Library.Payload
{
    public class SiLobbyInfo
    {
        public string Name { get; set; } = string.Empty;
        public int WaitingCount { get; set; }
        public bool IsHeadless { get; set; }
        public int? RemainingSecondsUntilAutoStart { get; set; }
        public List<SiLobbyConnection> Connections { get; set; } = new();
    }
}
