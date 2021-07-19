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
        private TcpListener _listener;
        public QpangServer Server;
        public byte[] KeyPart;
        public byte Encryption;
        private Player _player;
        private NetworkStream _socketStream;

        public int HeaderSizeToRead;
        public byte[] HeaderBuffer;
        public int PayloadSizeToRead;
        public byte[] PayloadBuffer;

        public Player Player
        {
            get { return this._player; }
            set { this._player = value; }
        }

        public ConnServer(Socket socket)
        {
            // reset
            PayloadBuffer = new byte[0];
            HeaderBuffer = new byte[4];
            HeaderSizeToRead = 4;

            this._socket = socket;
        }

        public void Read()
        {
            try
            {
                _socket.BeginReceive(HeaderBuffer, 0, HeaderSizeToRead, 0, ReceiveCallback, null);
                void ReceiveCallback(IAsyncResult ar)
                {
                    try
                    {
                        var bytesRead = _socket.EndReceive(ar);
                        if (bytesRead < 1)
                        {
                            if (Settings.DEBUG)
                                Log.Message(LogType.DUMP, $"[{_socket.LocalEndPoint}] Connection was lost during receiving!\n");
                            CloseSocket();
                            return;
                        }

                        if (HeaderSizeToRead > 0) // reading more data
                        {
                            HeaderSizeToRead -= bytesRead;
                            if (HeaderSizeToRead == 0)
                            {
                                if (OnHeaderReceived())
                                    _socket.BeginReceive(PayloadBuffer, 0, PayloadSizeToRead, 0, ReceiveCallback, null);
                                else
                                {
                                    if (Settings.DEBUG)
                                        Log.Message(LogType.DUMP, $"[{_socket.LocalEndPoint}] Invalid payload size!\n");
                                    CloseSocket();
                                }
                            }
                            else
                                _socket.BeginReceive(HeaderBuffer, HeaderBuffer.Length - HeaderSizeToRead, HeaderSizeToRead, 0, ReceiveCallback, null); // continue reading header
                        }
                        else
                        {
                            PayloadSizeToRead -= bytesRead;
                            if (PayloadSizeToRead == 0) // fully body!
                            {
                                PacketReader pkt = new PacketReader(HeaderBuffer, PayloadBuffer, "test", KeyPart);
                                if (Enum.IsDefined(typeof(Opcode), pkt.Opcode))
                                    if (Settings.DEBUG)
                                        Log.Message(LogType.DUMP, $"[{_socket.LocalEndPoint}] Recieved OpCode: {pkt.Opcode}, len: {pkt.Size}\n");
                                    else
                                        Log.Message(LogType.DUMP, $"[{_socket.LocalEndPoint}] Unknown OpCode: {pkt.Opcode}, len: {pkt.Size}\n");
                                else
                                    Log.Message(LogType.ERROR, $"[{_socket.LocalEndPoint}] Unregistered OpCode: {pkt.Opcode}\n");
                                PacketManager.InvokeHandler(pkt, this, pkt.Opcode);

                                // reset
                                PayloadBuffer = new byte[0];
                                HeaderBuffer = new byte[4];
                                HeaderSizeToRead = 4;
                                _socket.BeginReceive(HeaderBuffer, 0, HeaderSizeToRead, 0, ReceiveCallback, null); // start reading header
                            }
                            else
                                _socket.BeginReceive(PayloadBuffer, PayloadBuffer.Length - PayloadSizeToRead, PayloadSizeToRead, 0, ReceiveCallback, null); // continue reading payload
                        }
                    }
                    catch { }
                }
            }
            catch (Exception e)
            {
                // Shutup & be gone!
                Log.Message(LogType.ERROR, e.ToString());
                CloseSocket();
            }
        }

        public bool OnHeaderReceived()
        {
            PayloadSizeToRead = BitConverter.ToUInt16(HeaderBuffer, 0)-4;

            if (PayloadSizeToRead <= 0 || PayloadSizeToRead > 0xFFFF)
                return false;

            PayloadBuffer = new byte[PayloadSizeToRead];
            return true;
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
