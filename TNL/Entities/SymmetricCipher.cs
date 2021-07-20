using System;

namespace TNL.Entities
{
    using Utils;

    // TODO: System.Security.Cryptography.Rijndael / System.Security.Cryptography.RijndaelManaged

    public class SymmetricCipher
    {
        public const int BlockSize = 16;
        public const int KeySize = 16;

        private uint[] _counter = new uint[4];
        private uint[] _initVector = new uint[16];
        private byte[] _pad = new byte[16];
        private Key _symmetricKey = new();
        private uint _padLen;

        public SymmetricCipher(ByteBuffer theByteBuffer)
        {
            if (theByteBuffer.GetBufferSize() != KeySize * 2)
            {
                var buffer = new byte[KeySize];

                throw new NotImplementedException();

                Array.Copy(buffer, _initVector, BlockSize);
            }
            else
            {
                throw new NotImplementedException();

                Array.Copy(theByteBuffer.GetBuffer(), KeySize, _initVector, 0, BlockSize);
            }

            Array.Copy(_initVector, _counter, BlockSize);

            _padLen = 0;
        }

        public SymmetricCipher(byte[] symmetricKey, byte[] initVector)
        {
            Array.Copy(initVector, _initVector, BlockSize);
            Array.Copy(initVector, _counter, BlockSize); // Invalid Write in the Original TNL code i guess

            throw new NotImplementedException();

            _padLen = 0;
        }

        public void SetupCounter(uint counterValue1, uint counterValue2, uint counterValue3, uint counterValue4)
        {
            _counter[0] = _initVector[0] + counterValue1;
            _counter[1] = _initVector[1] + counterValue2;
            _counter[2] = _initVector[2] + counterValue3;
            _counter[3] = _initVector[3] + counterValue4;

            throw new NotImplementedException();

            _padLen = 0;
        }

        public void Encrypt(byte[] plainText, uint plainTextOffset, byte[] cipherText, uint cipherTextOffset, uint len)
        {
            while (len-- > 0)
            {
                if (_padLen == 16)
                {
                    throw new NotImplementedException();
                    _padLen = 0;
                }

                var encryptedChar = (byte) (plainText[plainTextOffset++] ^ _pad[_padLen]);
                _pad[_padLen++] = cipherText[cipherTextOffset++] = encryptedChar;
            }
        }

        public void Decrypt(byte[] plainText, uint plainTextOffset, byte[] cipherText, uint cipherTextOffset, uint len)
        {
            while (len-- > 0)
            {
                if (_padLen == BlockSize)
                {
                    throw new NotImplementedException();
                    _padLen = 0;
                }

                var encryptedChar = cipherText[cipherTextOffset++];
                plainText[plainTextOffset++] = (byte) (encryptedChar ^ _pad[_padLen]);
                _pad[_padLen++] = encryptedChar;
            }
        }

        private class Key
        {
            public uint[] Ek = new uint[64];
            public uint[] Dk = new uint[64];

            public int Nr { get; set; }
        }
    }
}
