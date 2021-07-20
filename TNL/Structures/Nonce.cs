using System;

namespace TNL.Structures
{
    using Utils;

    public class Nonce
    {
        public const int NonceSize = 8;

        public byte[] Data { get; private set; }

        public Nonce()
        {
            Data = new byte[NonceSize];
        }

        public Nonce(byte[] data)
            : this()
        {
            Array.Copy(data, Data, NonceSize);
        }

        public static bool operator ==(Nonce a, Nonce b)
        {
            return !(a is null) && !(b is null) && BitConverter.ToUInt64(a.Data, 0) == BitConverter.ToUInt64(b.Data, 0);
        }

        public static bool operator !=(Nonce a, Nonce b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var other = obj as Nonce;
            if (other == null)
                return false;

            return this == other;
        }

        public override int GetHashCode()
        {
            return (Data != null ? Data.GetHashCode() : 0);
        }

        public void Read(BitStream stream)
        {
            stream.Read(NonceSize, Data);
        }

        public void Write(BitStream stream)
        {
            stream.Write(NonceSize, Data);
        }

        public void GetRandom()
        {
            RandomUtil.Read(Data, NonceSize);
        }
    }
}
