using NTDLS.DatagramMessaging;
using NTDLS.ReliableMessaging;
using Si.Library.Messages.Notify.Datagram;

namespace Si.MultiplayClient
{
    internal class DatagramMessageNotificationHandlers : IDmMessageHandler
    {
        private readonly EngineMultiplayManager _manager;

        public DatagramMessageNotificationHandlers(EngineMultiplayManager manager)
        {
            _manager = manager;
        }

        public void OnSiSpriteActions(DmContext context, SiSpriteActions param)
        {
            _manager.InvokeApplySpriteActions(param);
        }
    }
}
