using NTDLS.TightRPC;
using Si.Library.Messages.Query;

namespace Si.MultiplayClient
{
    internal class RpcClientQueryHandlers : ITightRpcMessageHandler
    {
        private readonly EngineMultiplayManager _manager;

        public RpcClientQueryHandlers(EngineMultiplayManager manager)
        {
            _manager = manager;
        }

        public SiPingReply OnSiPing(TightRpcContext context, SiPing param)
        {
            return new SiPingReply(param);
        }
    }
}
