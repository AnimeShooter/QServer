using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.Webserver.Websocket
{
    public enum CMSGOpcode
    {
        CMSG_AUTH = 0,
        CMSG_HANDSHAKE = 1,
        CMSG_KEYEXCHANGE = 2,
        CMSG_LOGIN = 3,
        CMSG_REGISTER = 4,
        CMSG_STATS = 5,
        CMSG_PING = 6
    }

    public enum SMSGOpcode
    {
        SMSG_PONG = 0,
        SMSG_AUTH_RSP,
        SMSG_HANDSHAKE_STATUS,
        SMSG_LOGIN_RSP,
        SMSG_LOGIN_FAIL,
        SMSG_STATS_RSP
    }

    public class PPacket
    {
        public CMSGOpcode OpCode;
        public ulong Length;
        public byte[] Message;
    }
}
