using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using TNL.Entities;
using TNL.Utils;

namespace Qserver.GameServer.Qpang
{
    public class GameNetInterface : NetInterface
    {
        public GameNetInterface(int port) : base(port)
        {
            SetAllowConnections(true);
        }

        public override void ProcessPacket(IPEndPoint sourceAddress, BitStream stream)
        {
            if (sourceAddress == null)
                return; // ??
            base.ProcessPacket(sourceAddress, stream);
        }

        public override void HandleInfoPacket(IPEndPoint address, byte packetType, BitStream bitStream)
        {
            switch((PacketType)packetType)
            {
                case PacketType.FirstValidInfoPacketId:
                    PacketStream pingResponse = new PacketStream();

                    pingResponse.Write((byte)9);
                    pingResponse.WriteInt(5, 32);
                    pingResponse.WriteInt(6, 32);
                    pingResponse.SendTo(base.Socket, address);
                    break;
            }
        }
    }
}
