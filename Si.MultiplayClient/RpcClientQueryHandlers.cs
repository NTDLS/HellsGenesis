using NTDLS.ReliableMessaging;
using Si.Library.Messages.Query;

namespace Si.MultiplayClient
{
    internal class RpcClientQueryHandlers : IRmMessageHandler
    {
        private readonly EngineMultiplayManager _manager;

        public RpcClientQueryHandlers(EngineMultiplayManager manager)
        {
            _manager = manager;
        }

        public SiPingReply OnSiPing(RmContext context, SiPing param)
        {
            return new SiPingReply(param);
        }
    }
}
