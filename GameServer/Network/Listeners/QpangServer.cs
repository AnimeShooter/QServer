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
                _listener = new TcpListener(IPAddress.Parse(Settings.SERVER_IP), this._port);
                _listener.Start();

                // init multi thread accepter
                new Thread(AcceptConnection).Start(); 

                return true;
            }
            catch (Exception e)
            {
                Log.Message(LogType.ERROR, "{0}", e.Message);
                return false;
            }
        }

        protected void AcceptConnection()
        {
            try
            {
                while (ListenServerSocket)
                {
                    Thread.Sleep(1);
                    if (_listener.Pending())
                    {
                        // create new thread listener
                        ConnServer Server = new ConnServer(_listener.AcceptSocket());// .AcceptSocket());
                        new Thread(Server.OnReceive).Start();
                    }
                }
            }
            catch (Exception e)
            {
                // We do not want our server to crash due to a invalid packet beeing sent
                Console.WriteLine(e.ToString());
            }
        }

        protected void Dispose()
        {
            ListenServerSocket = false;
            _listener.Stop();
        }
    }
}
