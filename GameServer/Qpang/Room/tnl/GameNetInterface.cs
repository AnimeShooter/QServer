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
        public GameNetInterface(IPEndPoint a) : base(a.Port)
        {
            SetAllowConnections(true);
        }

        //public void ProcessPacket(IPEndPoint address, BitStream bitStream)
        //{
        //    ProcessPacket(address, bitStream);
        //}

        public void HandleInfoPacket(IPEndPoint address, PacketType packetType, BitStream bitStream)
        {
            switch(packetType)
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
