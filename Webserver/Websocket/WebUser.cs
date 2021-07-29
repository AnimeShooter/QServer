using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using Qserver.GameServer;

namespace Qserver.Webserver.Websocket
{
    // NOTE: our web clients that we can invoke commands on
    public class WebUser
    {
        public string Username;
        public string PasswordHash;
        public string RemoteAddress; //ip
        public DateTime ConnectedAt;
        public DateTime LastHeartbeat;
        private NetClient _NetClient;
        public NetClient NetClient
        {
            get { return this._NetClient; }
        }

        public WebUser()
        {
        }

        public WebUser(NetClient client)
        {
            this._NetClient = client;
        }

        public override string ToString()
        {
            return $"[{RemoteAddress}]: {Username} - {LastHeartbeat} ({ConnectedAt})";
        }

        public void SendPong()
        {
            _NetClient.Write((byte)SMSGOpcode.SMSG_PONG, new byte[] { 0x41 });
        }

        public void SendStats()
        {

        }
    }
}
