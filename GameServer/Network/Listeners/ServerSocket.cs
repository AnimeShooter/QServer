using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Qserver.Util;

namespace Qserver.GameServer.Network
{
    public class ServerSocket
    {
        public bool ListenServerSocket = true;
        private TcpListener _authListener;
        private TcpListener _parkListeren;
        private TcpListener _squareListener;

        public bool Start()
        {
            try
            {
#if DEBUG
                _authListener = new TcpListener(IPAddress.Parse(Settings.SERVER_IP), Settings.SERVER_PORT_AUTH);
                _parkListeren = new TcpListener(IPAddress.Parse(Settings.SERVER_IP), Settings.SERVER_PORT_PARK);
                _squareListener = new TcpListener(IPAddress.Parse(Settings.SERVER_IP), Settings.SERVER_PORT_SQUARE);
#else
                _authListener = new TcpListener(Util.Util.GetLocalIPAddress(), Settings.SERVER_PORT_AUTH);
#endif
                _authListener.Start();
                _parkListeren.Start();
                //_squareListener.Start();

                return true;
            }
            catch (Exception e)
            {
                Log.Message(LogType.ERROR, "{0}", e.Message);
                return false;
            }
        }

        public void StartConnectionThreads()
        {
            new Thread(AcceptAuthConnection).Start();
            new Thread(AcceptParkConnection).Start();
           //new Thread(AcceptSquareConnection).Start();
        }

        protected void AcceptAuthConnection()
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

                        Thread NewThread = new Thread(Server.RecieveAuth);
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

        protected void AcceptParkConnection()
        {
            try
            {
                while (ListenServerSocket)
                {
                    Thread.Sleep(1);
                    if (_parkListeren.Pending())
                    {
                        ServerManager Server = new ServerManager();
                        Server.ParkSocket = _parkListeren.AcceptSocket();

                        Thread NewThread = new Thread(Server.RecievePark);
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
