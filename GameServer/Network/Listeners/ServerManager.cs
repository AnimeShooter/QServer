using System;
using System.Net.Sockets;
using System.Threading;
using Qserver.Util;
using Qserver.GameServer.Network.Packets;
using Qserver.GameServer.Packets;
using Qserver.GameServer.Network.Managers;
using System.Threading.Tasks;
using Qserver.GameServer.Helpers;
using Qserver.GameServer.Network;

namespace Qserver.GameServer.Network
{
    public class ServerManager
    {
        public ulong Id;
        public Socket Socket;
        public Socket ParkSocket;
        public Socket SquareSocket;
        public static ServerSocket ServerSession;
        byte[] buffer = null;
        public byte[] KeyPart;
        public byte Encryption;

        public void OnData()
        {
            PacketReader pkt = new PacketReader(buffer, Socket.RemoteEndPoint.ToString(), KeyPart);
            if (Enum.IsDefined(typeof(Opcode), pkt.Opcode))
                Log.Message(LogType.DUMP, $"[] Recieved OpCode: {pkt.Opcode}, len: {pkt.Size}\n");
            else
                Log.Message(LogType.DUMP, $"[] Unknown OpCode: {pkt.Opcode}, len: {pkt.Size}\n");

            PacketManager.InvokeHandler(pkt, this, pkt.Opcode);
        }

        public void RecieveAuth()
        {
            try
            {
                Log.Message(LogType.MISC, "New Client Login Detected");
                while (ServerSession.ListenServerSocket)
                {
                    Thread.Sleep(1);
                    if (Socket.Connected && Socket.Available > 0)
                    {
                        buffer = new byte[Socket.Available];
                        Socket.Receive(buffer, buffer.Length, SocketFlags.None);
                        OnData();
                    }
                }

                CloseSocket();
            }
            catch (Exception e)
            {
                // Shutup & be gone!
                CloseSocket();
            }
        }

        // TODO: add scaling?
        public void RecievePark()
        {
            try
            {
                Log.Message(LogType.MISC, "New Client Park Detected");
                while (ServerSession.ListenServerSocket)
                {
                    Thread.Sleep(1);
                    if (ParkSocket.Connected && ParkSocket.Available > 0)
                    {
                        buffer = new byte[ParkSocket.Available];
                        ParkSocket.Receive(buffer, buffer.Length, SocketFlags.None);
                        OnData();
                    }
                }
                CloseSocket();
            }
            catch (Exception e)
            {
                // Shutup & be gone!
                CloseSocket();
            }
        }

        public void RecieveSquare()
        {
            try
            {
                Log.Message(LogType.MISC, "New Client Park Detected");
                while (ServerSession.ListenServerSocket)
                {
                    Thread.Sleep(1);
                    if (SquareSocket.Connected && SquareSocket.Available > 0)
                    {
                        buffer = new byte[SquareSocket.Available];
                        SquareSocket.Receive(buffer, buffer.Length, SocketFlags.None);
                        OnData();
                    }
                }
                CloseSocket();
            }
            catch (Exception e)
            {
                // Shutup & be gone!
                CloseSocket();
            }
        }

        public void Send(PacketWriter packet, bool SuppressLog = false, bool isAck = false)
        {
            if (packet == null)
                return;

            byte[] buffer = packet.ReadDataToSend(KeyPart);

            try
            {
                Socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(FinishSend), Socket);
                Log.Message(LogType.DUMP, $"Send {packet.Opcode}.\n");
                string bytes = "";
                foreach (var b in buffer)
                    bytes += b.ToString("X2") + " ";
                Log.Message(LogType.DUMP, bytes + "\n");
            }
            catch (Exception ex)
            {
                Log.Message(LogType.ERROR, "{0}", ex.Message);
                CloseSocket();
            }
        }

        public void CloseSocket()
        {
            Socket.Close();
        }

        public void FinishSend(IAsyncResult result)
        {
            Socket.EndSend(result);
        }
    }
}
