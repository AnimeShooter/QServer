using System;

namespace TNL.Utils
{
    using Entities;

    public class ByteBuffer : BaseObject
    {
        public const uint DefaultBufferSize = 1500U;

        protected byte[] Data;
        protected uint BufSize;

        public ByteBuffer(byte[] data, uint bufferSize)
        {
            BufSize = bufferSize;
            Data = data;
        }

        public ByteBuffer(uint bufferSize = DefaultBufferSize)
        {
            BufSize = bufferSize;
            Data = new byte[BufSize];
        }

        public void SetBuffer(byte[] data, uint bufferSize)
        {
            Data = data;
            BufSize = bufferSize;
        }

        public bool Resize(uint newBufferSize)
        {
            if (BufSize >= newBufferSize)
            {
                BufSize = newBufferSize;
            }
            else
            {
                BufSize = newBufferSize;
                Array.Resize(ref Data, (int) newBufferSize);
                return true;
            }

            return false;
        }

        public bool AppendBuffer(byte[] dataBuffer, uint bufferSize)
        {
            var start = BufSize;

            if (!Resize(BufSize + bufferSize))
                return false;

            Array.Copy(dataBuffer, 0, Data, start, bufferSize);
            return true;
        }

        public bool AppendBuffer(ByteBuffer theBuffer)
        {
            return AppendBuffer(theBuffer.GetBuffer(), theBuffer.GetBufferSize());
        }

        public uint GetBufferSize()
        {
            return BufSize;
        }

        public byte[] GetBuffer()
        {
            return Data;
        }

        public void Clear()
        {
            for (var i = 0; i < BufSize; ++i)
                Data[i] = 0;
        }
    }
}
