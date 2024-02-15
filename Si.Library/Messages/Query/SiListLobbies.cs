using NTDLS.ReliableMessaging;
using Si.Library.Payload;

namespace Si.Library.Messages.Query
{
    /// <summary>
    /// Requests a list of lobbies from the server.
    /// </summary>
    public class SiListLobbies : IRmQuery<SiListLobbiesReply>
    {
        //TODO: include some types of filters here
    }

    public class SiListLobbiesReply : IRmQueryReply
    {
        public List<SiLobbyConfiguration> Collection { get; set; } = new();
    }
}
