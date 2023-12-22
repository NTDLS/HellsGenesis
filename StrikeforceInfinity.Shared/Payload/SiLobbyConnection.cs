namespace StrikeforceInfinity.Shared.Payload
{
    public class SiLobbyConnection
    {
        /// <summary>
        /// The name the player gave themselves when joining.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The ping from the server to the client.
        /// </summary>
        public double LatencyMs { get; set; }

        /// <summary>
        /// The player has selected a loadout and is waiting on the server to start the game.
        /// </summary>
        public bool IsWaitingInLobby { get; set; }
    }
}