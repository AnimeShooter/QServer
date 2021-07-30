using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

// debug
using System.IO;

namespace TNL.Network
{
    public enum NetError
    {
        NoError,
        InvalidPacketProtocol,
        WouldBlock,
        UnknownError
    }

    public class TNLSocket
    {
        public const uint MaxPacketDataSize = 1490;

        private bool _needRun;
        private readonly UdpClient _socket;

        public Queue<Tuple<IPEndPoint, byte[]>> PacketsToBeHandled = new();

        private static object flock = new object();

        public TNLSocket()
        {
            _socket = new UdpClient();
        }

        public TNLSocket(int port)
        {
            _socket = new UdpClient(port);
            _socket.BeginReceive(OnEndReceive, null);

            _needRun = true;
        }

        private void OnEndReceive(IAsyncResult result)
        {
            try
            {
                var ep = new IPEndPoint(0, 0);

                var buff = _socket.EndReceive(result, ref ep);

                //// debug
                //lock(flock)
                //{
                //    string bytes = BitConverter.ToString(buff);
                //    if (!File.Exists("debug.txt"))
                //        File.Create("debug.txt").Close();
                //    string old = File.ReadAllText("debug.txt");
                //    File.WriteAllText("debug.txt", old + bytes + "\r\n");
                //}
               

                if (buff != null && buff.Length > 0)
                    PacketsToBeHandled.Enqueue(new(ep, buff));
            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine("Socket closed, stop listening!");

                Stop();
            }
            catch (SocketException se)
            {
                if (se.SocketErrorCode != SocketError.ConnectionReset)
                {
                    Console.WriteLine("Unknown error (receive)!");
                    Console.WriteLine(se);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown error (receive)!");
                Console.WriteLine(e);
            }

            if (_needRun && _socket != null)
                _socket.BeginReceive(OnEndReceive, null);
        }

        public void Stop()
        {
            _needRun = false;
        }

        public NetError Send(IPEndPoint iep, byte[] buffer, uint bufferSize)
        {
            try
            {
                //// debug
                //lock (flock)
                //{
                //    string bytes = BitConverter.ToString(buffer, 0, (int)bufferSize);
                //    if (!File.Exists("debug.txt"))
                //        File.Create("debug.txt").Close();
                //    string old = File.ReadAllText("debug.txt");
                //    File.WriteAllText("debug.txt", old + bytes + "\r\n");
                //}

                _socket.BeginSend(buffer, (int) bufferSize, iep, OnEndSend, null);

                return NetError.NoError;
            }
            catch
            {
                return NetError.UnknownError;
            }
        }

        public void OnEndSend(IAsyncResult result)
        {
            try
            {
                _socket.EndSend(result);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown error (send)!");
                Console.WriteLine(e);
            }
        }

        public void Connect(IPEndPoint ep)
        {
            _socket.Connect(ep);
            _socket.BeginReceive(OnEndReceive, null);

            _needRun = true;
        }
    }
}
