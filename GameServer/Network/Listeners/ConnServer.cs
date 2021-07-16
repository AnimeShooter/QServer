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
        private Socket _socket;
        public QpangServer Server;
        public byte[] KeyPart;
        public byte Encryption;
        private Player _player;
        private NetworkStream _socketStream;

        public Player Player
        {
            get { return this._player; }
            set { this._player = value; }
        }

        public ConnServer(Socket socket)
        {
            this._socket = socket;
            this._socketStream = new NetworkStream(this._socket);
        }

        public void OnReceive()
        {
#if !DEBUG
            try
            {
#endif
            Log.Message(LogType.MISC, "New Client Login Detected");
            while (_socket.Connected) // exit when closed?
            {
                Thread.Sleep(1);
                if (_socket.Connected && _socket.Available > 0)
                {
                    PacketReader pkt = new PacketReader(_socketStream, "test", KeyPart);
                    if (Enum.IsDefined(typeof(Opcode), pkt.Opcode))
                        if (Settings.DEBUG)
                            Log.Message(LogType.DUMP, $"[{_socket.LocalEndPoint}] Recieved OpCode: {pkt.Opcode}, len: {pkt.Size}\n");
                        else
                            Log.Message(LogType.DUMP, $"[{_socket.LocalEndPoint}] Unknown OpCode: {pkt.Opcode}, len: {pkt.Size}\n");
                    else
                        Log.Message(LogType.ERROR, $"[{_socket.LocalEndPoint}] Unregistered OpCode: {pkt.Opcode}\n");
                    PacketManager.InvokeHandler(pkt, this, pkt.Opcode);
                }
            }
            CloseSocket();
#if !DEBUG
            }
            catch (Exception e)
            {
                // Shutup & be gone!
                Log.Message(LogType.ERROR, e.ToString());
                CloseSocket();
            }
#endif
        }

        public void Send(PacketWriter packet, bool SuppressLog = false, bool isAck = false)
        {
            if (packet == null)
                return;

            byte[] buffer = packet.ReadDataToSend(KeyPart);

            try
            {
                _socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(FinishSend), _socket);
                if (Settings.DEBUG)
                { 
                    Log.Message(LogType.DUMP, $"Send {packet.Opcode} ({buffer.Length}).\n");
                    string bytes = "";
                    foreach (var b in buffer)
                        bytes += b.ToString("X2") + " ";
                    Log.Message(LogType.DUMP, bytes + "\n");
                }
            }
            catch (Exception ex)
            {
                Log.Message(LogType.ERROR, "{0}", ex.Message);
                CloseSocket();
            }
        }

        public void CloseSocket()
        {
            _socket.Close();
            if(this._player != null)
                this._player.Close();
        }

        public void FinishSend(IAsyncResult result)
        {
            _socket.EndSend(result);
        }
    }
}
