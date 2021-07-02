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
        public static AuthSocket ServerSession;
        byte[] buffer = null;
        public byte[] KeyPart;

        public void OnData()
        {
            PacketReader pkt = new PacketReader(buffer, Socket.RemoteEndPoint.ToString());
                if (Enum.IsDefined(typeof(Opcode), pkt.Opcode))
                    Log.Message(LogType.DUMP, $"[] Recieved OpCode: {pkt.Opcode}, len: {pkt.Size}\n");
                else
                    Log.Message(LogType.DUMP, $"[] Unknown OpCode: {pkt.Opcode}, len: {pkt.Size}\n");

            PacketManager.InvokeHandler(pkt, this, pkt.Opcode);
        }

        public void Recieve()
        {
            try
            {
                //bool d2 = true;
                //PacketWriter pw = new PacketWriter(ServerOpcode.D2GS_STARTLOGON);
                //pw.WriteUInt8(0x00); // no encryption
                //if (Socket.Connected)
                //    this.Send(pw);

                //Thread.Sleep(750);
                //if (Socket.Connected && Socket.Available == 0)
                //{
                //    d2 = false;
                //    pw = new PacketWriter(ServerOpcode.D2GS_STARTLOGON);
                //    pw.WriteUInt8(0x08); // no encryption i guess
                //    pw.WriteBytes(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00 }); // D2R detected
                //    this.Send(pw);
                //}

                Log.Message(LogType.MISC, "New Client detected");
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
            catch(Exception e)
            {
                // Shutup & be gone!
                CloseSocket();
            }
        }

        public void Send(PacketWriter packet, bool SuppressLog = false, bool isAck = false)
        {
            if (packet == null)
                return;

            byte[] buffer = packet.ReadDataToSend();

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
