using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Qserver.Util;
using System.Collections.Generic;

namespace Qserver.GameServer.Network
{
    public class QpangServer
    {
        public bool ListenServerSocket = true;
        public TcpListener _listener;
        public int _port;
        private Dictionary<uint, ConnServer> _connections;

        public QpangServer(int port) // used for auth?
        {
            this._port = port;
        }

        public bool Start()
        {
            try
            {
                new Thread(handleNew).Start();
                void handleNew()
                {
                    _listener = new TcpListener(IPAddress.Parse(Settings.SERVER_IP), this._port);
                    _listener.Start();
                    while (true)
                    {
                        Thread.Sleep(1);
                        if(_listener.Pending())
                            _listener.BeginAcceptSocket(AcceptCallback, null);
                    }
                            
                }
                return true;
            }
            catch (Exception e)
            {
                Log.Message(LogType.ERROR, "{0}", e.Message);
                return false;
            }
        }

        void AcceptCallback(IAsyncResult ar)
        {
            var conn = new ConnServer(_listener.EndAcceptSocket(ar));
            conn.Read();
        }


        protected void Dispose()
        {
            ListenServerSocket = false;
            _listener.Stop();
        }
    }
}
