using System;
using System.Security.Cryptography;

namespace TNL.Entities
{
    using Utils;

    public enum KeyType
    {
        KeyTypePrivate,
        KeyTypePublic
    }

    public class AsymmetricKey : ByteBuffer
    {
        public const uint StaticCryptoBufferSize = 2048U;

        private static readonly byte[] StaticCryptoBuffer = new byte[StaticCryptoBufferSize];

        private byte[] KeyData { get; set; }
        private uint KeySize { get; set; }
        private bool PHasPrivateKey { get; set; }
        private ByteBuffer PublicKey { get; set; }
        private ByteBuffer PrivateKey { get; set; }
        private bool PIsValid { get; set; }

        public AsymmetricKey(byte[] buffer, uint bufferSize)
        {
            Load(new ByteBuffer(buffer, bufferSize));
        }

        public AsymmetricKey(BitStream stream)
        {
            var theBuffer = new ByteBuffer();

            stream.Read(theBuffer);

            Load(theBuffer);
        }

        public AsymmetricKey(uint keySize)
        {
            PIsValid = false;

            KeySize = keySize;

            throw new NotImplementedException();
        }

        private void Load(ByteBuffer theBuffer)
        {
            PIsValid = false;
            PHasPrivateKey = theBuffer.GetBuffer()[0] == (byte) KeyType.KeyTypePrivate;

            var bufferSize = theBuffer.GetBufferSize();
            if (bufferSize < 5)
                return;

            var temp = new byte[4];
            Array.Copy(theBuffer.GetBuffer(), 1, temp, 0, 4);
            Array.Reverse(temp);

            KeySize = BitConverter.ToUInt32(temp, 0);

            throw new NotImplementedException();
        }

        public ByteBuffer GetPublicKey()
        {
            return PublicKey;
        }

        public ByteBuffer GetPrivateKey()
        {
            return PrivateKey;
        }

        public bool HasPrivateKey()
        {
            return PHasPrivateKey;
        }

        public bool IsValid()
        {
            return PIsValid;
        }

        public ByteBuffer ComputeSharedSecretKey(AsymmetricKey publicKey)
        {
            if (publicKey.GetKeySize() != GetKeySize() || !PHasPrivateKey)
                return null;

            throw new NotImplementedException();

            var hash = new SHA256Managed().ComputeHash(StaticCryptoBuffer, 0, (int) StaticCryptoBufferSize);

            return new ByteBuffer(hash, 32);
        }

        public uint GetKeySize()
        {
            return KeySize;
        }

        public ByteBuffer HashAndSign(ByteBuffer theByteBuffer)
        {
            throw new NotImplementedException();

            var hash = new SHA256Managed().ComputeHash(theByteBuffer.GetBuffer(), 0, (int) theByteBuffer.GetBufferSize());

            return new ByteBuffer(StaticCryptoBuffer, StaticCryptoBufferSize);
        }

        public bool VerifySignature(ByteBuffer theByteBuffer, ByteBuffer theSignature)
        {
            throw new NotImplementedException();

            var hash = new SHA256Managed().ComputeHash(theByteBuffer.GetBuffer(), 0, (int) theByteBuffer.GetBufferSize());

            return false;
        }
    }
}
