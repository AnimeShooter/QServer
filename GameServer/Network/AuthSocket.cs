using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Qserver.Util;

namespace Qserver.GameServer.Network
{
    public class AuthSocket
    {
        public bool ListenServerSocket = true;
        private TcpListener _authListener;

        public bool Start()
        {
            try
            {
#if DEBUG
                _authListener = new TcpListener(IPAddress.Parse(Settings.SERVER_IP), Settings.SERVER_PORT_PARK);
#else
                _authListener = new TcpListener(Util.Util.GetLocalIPAddress(), Settings.SERVER_PORT);
#endif
                _authListener.Start();

                return true;
            }
            catch (Exception e)
            {
                Log.Message(LogType.ERROR, "{0}", e.Message);
                return false;
            }
        }

        public void StartConnectionThread()
        {
            new Thread(AcceptConnection).Start();
        }

        protected void AcceptConnection()
        {
            try
            {
                while (ListenServerSocket)
                {
                    Thread.Sleep(1);
                    if (_authListener.Pending())
                    {
                        ServerManager Server = new ServerManager();
                        Server.Socket = _authListener.AcceptSocket();

                        Thread NewThread = new Thread(Server.Recieve);
                        NewThread.Start();
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
            _authListener.Stop();
        }
    }
}
