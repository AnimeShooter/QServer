using Qserver.GameServer.Network.Packets;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Qserver.Util;
using Qserver.GameServer.Packets;
using System.Threading;
using Qserver.GameServer.Qpang;

namespace Qserver.GameServer.Network
{
    public class ConnServer
    {
        public ulong Id;
        public Socket Socket;
        public QpangServer Server;
        public byte[] KeyPart;
        public byte Encryption;
        private Player _player;

        public Player Player
        {
            get { return this._player; }
            set { this._player = value; }
        }

        public void OnData(byte[] buffer)
        {
            PacketReader pkt = new PacketReader(buffer, "test", KeyPart);
            if (Enum.IsDefined(typeof(Opcode), pkt.Opcode))
                Log.Message(LogType.DUMP, $"[] Recieved OpCode: {pkt.Opcode}, len: {pkt.Size}\n");
            else
                Log.Message(LogType.DUMP, $"[] Unknown OpCode: {pkt.Opcode}, len: {pkt.Size}\n");

            PacketManager.InvokeHandler(pkt, this, pkt.Opcode);
        }


        public void RecieveAuth()
        {
            //try
            //{
                Log.Message(LogType.MISC, "New Client Login Detected");
                while (Server.ListenServerSocket)
                {
                    Thread.Sleep(1);
                    if (Socket.Connected && Socket.Available > 0)
                    {
                        byte[] buffer = new byte[Socket.Available];
                        Socket.Receive(buffer, buffer.Length, SocketFlags.None);
                        OnData(buffer);
                    }
                }

                CloseSocket();
            //}
            //catch (Exception e)
            //{
            //    // Shutup & be gone!
            //    Log.Message(LogType.ERROR, e.ToString());
            //    CloseSocket();
            //}
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
