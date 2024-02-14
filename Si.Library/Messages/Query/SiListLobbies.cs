using NTDLS.TightRPC;
using Si.Library.Payload;

namespace Si.Library.Messages.Query
{
    /// <summary>
    /// Requests a list of lobbies from the server.
    /// </summary>
    public class SiListLobbies : ITightRpcQuery<SiListLobbiesReply>
    {
        //TODO: include some types of filters here
    }

    public class SiListLobbiesReply : ITightRpcQueryReply
    {
        public List<SiLobbyConfiguration> Collection { get; set; } = new();
    }
}
