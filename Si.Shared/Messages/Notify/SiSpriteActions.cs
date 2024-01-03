﻿using NTDLS.UDPPacketFraming.Payloads;
using Si.Shared.Payload.DroneActions;

namespace Si.Shared.Messages.Notify
{
    /// <summary>
    /// Tells the server that a sprite has done something. Fire, move, explode, etc.
    /// </summary>
    public class SiSpriteActions : IUDPPayloadNotification
    {
        public Guid ConnectionId { get; set; }
        public List<SiSpriteAction> Collection { get; set; }

        public SiSpriteActions(List<SiSpriteAction> collection)
        {
            Collection = collection;
        }
    }
}
