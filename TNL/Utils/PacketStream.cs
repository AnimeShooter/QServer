using System;
using System.Net;

namespace TNL.Utils
{
    using Network;

    public class PacketStream : BitStream
    {
        private readonly byte[] _buffer = new byte[TNLSocket.MaxPacketDataSize];

        public PacketStream(uint targetPacketSize = TNLSocket.MaxPacketDataSize)
        {
            BufSize = targetPacketSize;
            Data = _buffer;

            SetMaxSizes(targetPacketSize, TNLSocket.MaxPacketDataSize);
            Reset();

            CurrentByte = new byte[1];
        }

        public NetError SendTo(TNLSocket outgoingSocket, IPEndPoint theAddress)
        {
            return outgoingSocket.Send(theAddress, _buffer, GetBytePosition());
        }

        public NetError RecvFrom(TNLSocket incomingSocket, out IPEndPoint recvAddress)
        {
            if (incomingSocket.PacketsToBeHandled.Count == 0)
            {
                recvAddress = null;
                return NetError.WouldBlock;
            }

            var d = incomingSocket.PacketsToBeHandled.Dequeue();

            var dataSize = d.Item2.Length > TNLSocket.MaxPacketDataSize ? TNLSocket.MaxPacketDataSize : (uint) d.Item2.Length;

            Array.Copy(d.Item2, _buffer, dataSize);

            SetBuffer(_buffer, dataSize);
            SetMaxSizes(dataSize, 0U);
            Reset();

            recvAddress = d.Item1;

            return NetError.NoError;
        }
    }
}
