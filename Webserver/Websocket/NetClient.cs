using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Qserver.Webserver.Websocket
{
    // NOTE: handles the response of clients when they ask for things
    public class NetClient
    {
        private CancellationTokenSource WorkTask_runCTok { get; }
        public TcpClient Client { get; }
        private NetServer Server { get; }
        public StreamReader ClientStreamReader { get; }
        public NetworkStream ClientStream { get; }

        public NetClient(TcpClient client, NetServer server)
        {
            this.Client = client;
            this.Server = server;
            this.WorkTask_runCTok = new CancellationTokenSource();
            this.ClientStream = client.GetStream();
            this.ClientStreamReader = new StreamReader(client.GetStream());
            Task.Run(HandleData);
        }

        public void DisconnectAndDispose()
        {
            try
            {
                CancelWorkThread();
                this.Client.Dispose();
            }
            catch (ObjectDisposedException) { }
        }

        private void CancelWorkThread()
        {
            this.WorkTask_runCTok.Cancel();
            this.Server.DisconnectingClients.Enqueue(this);
        }

        public async Task HandleData()
        {
            PerformHandshake();

            WebUser bot = null;
            lock (NetServer.Users)
                bot = NetServer.Users.Find(x => x.RemoteAddress == Client.Client.RemoteEndPoint.ToString());

            bot.SendStats();
            while (!this.WorkTask_runCTok.IsCancellationRequested)
            {
                var pp = await ReadPacketAsync();
                if (pp == null)
                    return;

                switch (pp.OpCode)
                {
                    case CMSGOpcode.CMSG_PING:
                        bot.LastHeartbeat = DateTime.UtcNow;
                        bot.SendPong();
                        break;
                    //case CMSGOpcode.CMSG_STATS:
                    //    bot.SendStats();
                    //    break;
                    default:
                        break;
                }
            }
        }

        private async Task<PPacket> ReadPacketAsync()
        {
            bool done = false;
            PPacket pp = new PPacket();
            while (!done)
            {
                var buffer = new byte[2];
                var bytesRead = await this.ClientStream.ReadAsync(buffer, 0, buffer.Length, this.WorkTask_runCTok.Token);
                if (buffer.Length == bytesRead)
                {
                    done = (buffer[0] & 0b1000_0000) != 0;
                    bool mask = (buffer[1] & 0b1000_0000) != 0;

                    int unkOpcode = buffer[0] & 0b0000_1111;

                    pp.Length = (ulong)(buffer[1] & 0b0111_1111); // remove mask bit

                    if (pp.Length == 126)
                    {
                        var buffer2 = new byte[2];
                        await this.ClientStream.ReadAsync(buffer2, 0, buffer2.Length, this.WorkTask_runCTok.Token);

                        pp.Length = BitConverter.ToUInt16(new byte[] { buffer2[1], buffer2[0] }, 0);
                    }
                    else if (pp.Length == 127)
                    {
                        var buffer2 = new byte[8];
                        await this.ClientStream.ReadAsync(buffer2, 0, buffer2.Length, this.WorkTask_runCTok.Token);
                        pp.Length = BitConverter.ToUInt64(new byte[] { buffer2[7], buffer2[6], buffer2[5], buffer2[4], buffer2[3], buffer2[2], buffer2[1], buffer2[0] }, 0);
                    }

                    if (mask)
                    {
                        var maskBuffer = new byte[4];
                        await this.ClientStream.ReadAsync(maskBuffer, 0, maskBuffer.Length, this.WorkTask_runCTok.Token);

                        byte[] msgBytes = new byte[pp.Length];
                        await this.ClientStream.ReadAsync(msgBytes, 0, msgBytes.Length, this.WorkTask_runCTok.Token);
                        pp.Message = new byte[pp.Length];
                       

                        for (ulong i = 0; i < pp.Length; ++i)
                            pp.Message[i] = (byte)(msgBytes[i] ^ maskBuffer[i % 4]);
                        pp.OpCode = (CMSGOpcode)pp.Message[0];

#if DEBUG
                        Console.WriteLine("Received message from Websocket.");
#endif
                        return pp;
                    }
                    else
                    {
#if DEBUG
                        Console.WriteLine("Mask bit was not set clientside.");
#endif
                        return pp;
                    }
                }
                else
                {
#if DEBUG
                    Console.WriteLine("Socket read was interrupted because a shutdown was requested.");
#endif
                    return null; // shutdown was requested
                }
            }
            return pp;
        }

        private void PerformHandshake()
        {
            string line = null;
            string key = "";
            string responseKey = "";

            string MAGIC_STRING = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
            while (line != "")
            {
                line = ClientStreamReader.ReadLine();
                if (line.StartsWith("Sec-WebSocket-Key:"))
                {
                    key = line.Split(':')[1].Trim();
                }
            }

            if (key != "")
            {
                key += MAGIC_STRING;
                using (var sha1 = SHA1.Create())
                {
                    responseKey = Convert.ToBase64String(sha1.ComputeHash(Encoding.ASCII.GetBytes(key)));
                }
            }

            // send handshake to the client
            var itemsToSend = new List<string>
            {
                "HTTP/1.1 101 Web Socket Protocol Handshake",
                "Upgrade: WebSocket",
                "Connection: Upgrade",
                "WebSocket-Origin: http://127.0.0.1",
                "WebSocket-Location: ws://localhost:8181/websession"
            };

            if (!string.IsNullOrEmpty(responseKey))
                itemsToSend.Add("Sec-WebSocket-Accept: " + responseKey);

            itemsToSend.Add("\n"); // double \n at the end!

            var bytes = Encoding.UTF8.GetBytes(string.Join("\n", itemsToSend));

            ClientStream.Write(bytes);
            ClientStream.Flush();
        }


        private readonly object sendLockObj = new object();
        public void Write(byte opcode, byte[] buffer)
        {
            List<byte> buf = new List<byte>();
            buf.Add(1); // byte
            buf.Add(opcode); // 1byte opcode
            buf.AddRange(buffer);

            lock (this.sendLockObj)
            {
                int frameSize = 64;
                var parts = buf.Select((b, i) => new { b, i })
                               .GroupBy(x => x.i / (frameSize - 1))
                               .Select(x => x.Select(y => y.b).ToArray())
                               .ToList();

                for (int i = 0; i < parts.Count; i++)
                {
                    byte cmd = 0;
                    if (i == 0) cmd |= 1;
                    if (i == parts.Count - 1) cmd |= 0x80;

                    this.ClientStream.WriteByte(cmd);
                    this.ClientStream.WriteByte((byte)parts[i].Length);
                    this.ClientStream.Write(parts[i], 0, parts[i].Length);
                }

                this.ClientStream.Flush();
            }
        }

        public async Task AcceptConnection(PPacket pp)
        {
            string[] data = Encoding.UTF8.GetString(pp.Message).Split(':');

            if (data.Length != 2)
                return;

            lock (NetServer.Users)
            {
                var bot = NetServer.Users.Find(x => x.RemoteAddress == this.Client.Client.RemoteEndPoint.ToString());
                bot.Username = data[0];
                bot.PasswordHash = data[1];
            }
        }
    }
}
