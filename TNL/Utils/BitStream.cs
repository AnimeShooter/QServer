using System;
using System.Text;
using System.Security.Cryptography;

namespace TNL.Utils
{
    using Entities;
    using Structures;
    using Types;

    public class BitStream : ByteBuffer
    {
        #region Consts
        public const float FloatOne = 1.0f;
        public const float FloatHalf = 0.5f;
        public const float FloatZero = 0.0f;

        public const float FloatPi = (float) Math.PI;
        public const float Float2Pi = 2.0f * FloatPi;
        public const float FloatInversePi = 1.0f / FloatPi;
        public const float FloatHalfPi = 0.5f * FloatPi;
        public const float Float2InversePi = 2.0f / FloatPi;
        public const float FloatInverse2Pi = 0.5f / FloatPi;

        public const float FloatSqrt2 = 1.41421356237309504880f;
        public const float FloatSqrtHalf = 0.7071067811865475244008443f;

        public static readonly byte[] BitCounts = { 16, 18, 20, 32 };

        #endregion

        public const uint ResizePad = 1500U;

        protected uint BitNum;
        protected bool Error;
        protected bool CompressRelative;
        protected Point3F CompressPoint;
        protected uint MaxReadBitNum;

        public uint MaxWriteBitNum;

        protected byte[] CurrentByte;
        protected ConnectionStringTable StringTable;
        protected readonly byte[] StringBuffer = new byte[256];

        public BitStream(byte[] data, uint bufSize)
            : base(data, bufSize)
        {
            SetMaxSizes(bufSize, bufSize);
            Reset();
            CurrentByte = new byte[1];
        }

        public BitStream(byte[] data, uint bufSize, uint maxWriteSize)
            : base(data, bufSize)
        {
            SetMaxSizes(bufSize, maxWriteSize);
            Reset();
            CurrentByte = new byte[1];
        }

        public BitStream()
        {
            SetMaxSizes(GetBufferSize(), GetBufferSize());
            Reset();
            CurrentByte = new byte[1];
        }

        protected bool ResizeBits(uint newBitsNeeded)
        {
            var newSize = ((MaxWriteBitNum + newBitsNeeded + 7) >> 3) + ResizePad;
            if (!Resize(newSize))
            {
                Error = true;
                return false;
            }

            MaxReadBitNum = newSize << 3;
            MaxWriteBitNum = newSize << 3;
            return true;
        }

        public void SetMaxSizes(uint maxReadSize, uint maxWriteSize)
        {
            MaxReadBitNum = maxReadSize << 3;
            MaxWriteBitNum = maxWriteSize << 3;
        }

        public void SetMaxBitSizes(uint maxReadBitSize, uint maxWriteBitSize)
        {
            MaxReadBitNum = maxReadBitSize;
            MaxWriteBitNum = maxWriteBitSize;
        }

        public void Reset()
        {
            BitNum = 0;
            Error = false;
            CompressRelative = false;
            StringBuffer[0] = 0;
            StringTable = null;
        }

        public void CleanStringBuffer()
        {
            StringBuffer[0] = 0;
        }

        public void SetStringTable(ConnectionStringTable table)
        {
            StringTable = table;
        }

        public void ClearError()
        {
            Error = false;
        }

        public uint GetBytePosition()
        {
            return (BitNum + 7) >> 3;
        }

        public uint GetBitPosition()
        {
            return BitNum;
        }

        public void SetBytePosition(uint newPosition)
        {
            BitNum = newPosition << 3;
        }

        public void SetBitPosition(uint newBitPosition)
        {
            BitNum = newBitPosition;
        }

        public void AdvanceBitPosition(uint numBits)
        {
            SetBitPosition(GetBitPosition() + numBits);
        }

        public uint GetMaxReadBitPosition()
        {
            return MaxReadBitNum;
        }

        public uint GetBitSpaceAvailable()
        {
            return MaxWriteBitNum - BitNum;
        }

        public void ZeroToByteBoundary()
        {
            if ((BitNum & 0x7) != 0)
                WriteInt(0, (byte) (8 - (BitNum & 0x7)));
        }

        public void WriteInt(uint value, byte bitCount)
        {
            WriteBits(bitCount, BitConverter.GetBytes(value));
        }

        public uint ReadInt(byte bitCount)
        {
            var bits = new byte[4];

            ReadBits(bitCount, bits);

            var ret = BitConverter.ToUInt32(bits, 0);

            if (bitCount == 32)
                return ret;

            ret &= (1U << bitCount) - 1;

            return ret;
        }

        public void WriteIntAt(uint value, byte bitCount, uint bitPosition)
        {
            var curPos = GetBitPosition();

            SetBitPosition(bitPosition);

            WriteInt(value, bitCount);

            SetBitPosition(curPos);
        }

        public void WriteSignedInt(int value, byte bitCount)
        {
            if (WriteFlag(value < 0))
                WriteInt((uint) (-value), (byte) (bitCount - 1));
            else
                WriteInt((uint) value, (byte) (bitCount - 1));
        }

        public int ReadSignedInt(byte bitCount)
        {
            if (ReadFlag())
                return -(int) ReadInt((byte) (bitCount - 1));

            return (int) ReadInt((byte) (bitCount - 1));
        }

        public void WriteRangedU32(uint value, uint rangeStart, uint rangeEnd)
        {
            var rangeSize = rangeEnd - rangeStart + 1;
            var rangeBits = Utils.GetNextBinLog2(rangeSize);

            WriteInt(value - rangeStart, (byte) rangeBits);
        }

        public uint ReadRangedU32(uint rangeStart, uint rangeEnd)
        {
            var rangeSize = rangeEnd - rangeStart + 1;
            var rangeBits = Utils.GetNextBinLog2(rangeSize);

            return ReadInt((byte) rangeBits) + rangeStart;
        }

        public void WriteEnum(uint enumValue, uint enumRange)
        {
            WriteInt(enumValue, (byte) Utils.GetNextBinLog2(enumRange));
        }

        public uint ReadEnum(uint enumRange)
        {
            return ReadInt((byte) Utils.GetNextBinLog2(enumRange));
        }

        public void WriteFloat(float f, byte bitCount)
        {
            WriteInt((uint) (f * ((1 << bitCount) - 1)), bitCount);
        }

        public float ReadFloat(byte bitCount)
        {
            return ReadInt(bitCount) / (float) ((1 << bitCount) - 1);
        }

        public void WriteSignedFloat(float f, byte bitCount)
        {
            WriteSignedInt((int) (f * ((1 << (bitCount - 1)) - 1)), bitCount);
        }

        public float ReadSignedFloat(byte bitCount)
        {
            return ReadSignedInt(bitCount) / (float) ((1 << (bitCount - 1)) - 1);
        }

        public void WriteClassId(uint classId, uint classType, uint classGroup)
        {
            WriteInt(classId, (byte) NetClassRep.GetNetClassBitSize(classGroup, classType));
        }

        public uint ReadClassId(uint classType, uint classGroup)
        {
            var ret = ReadInt((byte) NetClassRep.GetNetClassBitSize(classGroup, classType));
            return ret >= NetClassRep.GetNetClassCount(classGroup, classType) ? 0xFFFFFFFFU : ret;
        }

        public void WriteNormalVector(Point3F vec, byte bitCount)
        {
            var phi = (float) (Math.Atan2(vec.X, vec.Y) * FloatInversePi);
            var theta = (float) (Math.Atan2(vec.Z, Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y)) * Float2InversePi);

            WriteSignedFloat(phi, (byte) (bitCount + 1));
            WriteSignedFloat(theta, bitCount);
        }

        public void ReadNormalVector(ref Point3F vec, byte bitCount)
        {
            var phi = ReadSignedFloat((byte) (bitCount + 1)) * FloatPi;
            var theta = ReadSignedFloat(bitCount) * FloatHalfPi;

            vec.X = (float) (Math.Sin(phi) * Math.Cos(theta));
            vec.Y = (float) (Math.Cos(phi) * Math.Cos(theta));
            vec.Z = (float) Math.Sin(theta);
        }

        public static Point3F DumbDownNormal(Point3F vec, byte bitCount)
        {
            var buffer = new byte[128];
            var temp = new BitStream(buffer, 128U);

            temp.WriteNormalVector(vec, bitCount);
            temp.SetBitPosition(0U);

            var ret = new Point3F();

            temp.ReadNormalVector(ref ret, bitCount);

            return ret;
        }

        public void WriteNormalVector(Point3F vec, byte angleBitCount, byte zBitCount)
        {
            if (WriteFlag(Math.Abs(vec.Z) >= (1.0f - (1.0f / zBitCount))))
                WriteFlag(vec.Z < 0.0f);
            else
            {
                WriteSignedFloat(vec.Z, zBitCount);
                WriteSignedFloat((float) Math.Atan2(vec.X, vec.Y) * FloatInverse2Pi, angleBitCount);
            }
        }

        public void ReadNormalVector(ref Point3F vec, byte angleBitCount, byte zBitCount)
        {
            if (ReadFlag())
            {
                vec.Z = ReadFlag() ? -1.0f : 1.0f;
                vec.X = 0.0f;
                vec.Y = 0.0f;
            }
            else
            {
                vec.Z = ReadSignedFloat(zBitCount);

                var angle = Float2Pi * ReadSignedFloat(angleBitCount);

                var mult = (float) Math.Sqrt(1.0f - vec.Z * vec.Z);
                vec.X = mult * (float) Math.Cos(angle);
                vec.X = mult * (float) Math.Sin(angle);
            }
        }

        public void SetPointCompression(Point3F p)
        {
            CompressRelative = true;
            CompressPoint = p;
        }

        public void ClearPointCompression()
        {
            CompressRelative = false;
        }

        public void WritePointCompressed(Point3F p, float scale)
        {
            var vec = new Point3F();

            var invScale = 1.0f / scale;
            uint type;

            if (CompressRelative)
            {
                vec.X = p.X - CompressPoint.X;
                vec.Y = p.Y - CompressPoint.Y;
                vec.Z = p.Z - CompressPoint.Z;

                var dist = (float) Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y + vec.Z * vec.Z) * invScale;

                if (dist < (1 << 15))
                    type = 0U;
                else if (dist < (1 << 17))
                    type = 1U;
                else if (dist < (1 << 19))
                    type = 2U;
                else
                    type = 3U;
            }
            else
                type = 3U;

            WriteInt(type, 2);

            if (type != 3U)
            {
                var size = BitCounts[type];

                WriteSignedInt((int) (vec.X * invScale), size);
                WriteSignedInt((int) (vec.Y * invScale), size);
                WriteSignedInt((int) (vec.Z * invScale), size);
            }
            else
            {
                Write(p.X);
                Write(p.Y);
                Write(p.Z);
            }
        }

        public void ReadPointCompressed(ref Point3F p, float scale)
        {
            var type = ReadInt(2);
            if (type == 3)
            {
                Read(out float x);
                Read(out float y);
                Read(out float z);

                p.X = x;
                p.Y = y;
                p.Z = z;
            }
            else
            {
                var size = BitCounts[type];

                p.X = ReadSignedInt(size);
                p.Y = ReadSignedInt(size);
                p.Z = ReadSignedInt(size);

                p.X = CompressPoint.X + p.X * scale;
                p.Y = CompressPoint.Y + p.Y * scale;
                p.Z = CompressPoint.Z + p.Z * scale;
            }
        }

        public bool WriteBits(uint bitCount, byte[] bitPtr)
        {
            if (bitCount == 0)
                return true;

            if (bitCount + BitNum > MaxWriteBitNum && !ResizeBits(bitCount + BitNum - MaxWriteBitNum))
                return false;

            var upShift = BitNum & 0x7;
            var downShift = 8 - upShift;

            var sourcePtr = bitPtr;
            var sourceOff = 0;

            var destPtr = GetBuffer();
            var destOff = BitNum >> 3;

            if (downShift >= bitCount)
            {
                var mask = ((1 << (int) bitCount) - 1) << (int) upShift;

                destPtr[destOff] = (byte) ((destPtr[destOff] & ~mask) | ((sourcePtr[sourceOff] << (int) upShift) & mask));

                BitNum += bitCount;
                return true;
            }

            if (upShift == 0)
            {
                BitNum += bitCount;

                for (; bitCount >= 8; bitCount -= 8)
                    destPtr[destOff++] = sourcePtr[sourceOff++];

                if (bitCount > 0)
                {
                    var mask = (1 << (int) bitCount) - 1;
                    destPtr[destOff] = (byte) ((sourcePtr[sourceOff] & mask) | (destPtr[destOff] & ~mask));
                }

                return true;
            }

            byte sourceByte;
            var destByte = (byte) (destPtr[destOff] & (0xFF >> (int) downShift));
            var lastMask = (byte) (0xFF >> (int) (7 - ((BitNum + bitCount - 1) & 0x7)));

            BitNum += bitCount;

            for (; bitCount >= 8; bitCount -= 8)
            {
                sourceByte = sourcePtr[sourceOff++];

                destPtr[destOff++] = (byte) (destByte | (sourceByte << (int) upShift));

                destByte = (byte) (sourceByte >> (int) downShift);
            }

            if (bitCount == 0)
            {
                destPtr[destOff] = (byte) ((destPtr[destOff] & ~lastMask) | (destByte & lastMask));
                return true;
            }

            if (bitCount <= downShift)
            {
                destPtr[destOff] = (byte) ((destPtr[destOff] & ~lastMask) | ((destByte | (sourcePtr[sourceOff] << (int) upShift)) & lastMask));
                return true;
            }

            sourceByte = sourcePtr[sourceOff];

            destPtr[destOff++] = (byte) (destByte | (sourceByte << (int) upShift));
            destPtr[destOff] = (byte) ((destPtr[destOff] & ~lastMask) | ((sourceByte >> (int) downShift) & lastMask));
            return true;
        }

        public bool ReadBits(uint bitCount, byte[] bitPtr)
        {
            if (bitCount == 0)
                return true;

            if (bitCount + BitNum > MaxReadBitNum)
            {
                Error = true;
                return false;
            }

            var sourcePtr = GetBuffer();
            var sourceOff = BitNum >> 3;

            var byteCount = (bitCount + 7) >> 3;

            var destPtr = bitPtr;
            var destOff = 0;

            var downShift = BitNum & 0x7;
            var upShift = 8 - downShift;

            if (downShift == 0)
            {
                while (byteCount-- > 0)
                    destPtr[destOff++] = sourcePtr[sourceOff++];

                BitNum += bitCount;
                return true;
            }

            var sourceByte = (byte) (sourcePtr[sourceOff] >> (int) downShift);
            BitNum += bitCount;

            for (; bitCount >= 8; bitCount -= 8)
            {
                var nextByte = sourcePtr[++sourceOff];

                destPtr[destOff++] = (byte) (sourceByte | (nextByte << (int) upShift));

                sourceByte = (byte) (nextByte >> (int) downShift);
            }

            if (bitCount > 0)
            {
                if (bitCount <= upShift)
                {
                    destPtr[destOff] = sourceByte;
                    return true;
                }
                destPtr[destOff] = (byte) (sourceByte | (sourcePtr[++sourceOff] << (int) upShift));
            }

            return true;
        }

        public bool Write(ByteBuffer theBuffer)
        {
            var size = theBuffer.GetBufferSize();
            if (size > 1023)
                return false;

            WriteInt(size, 10);
            return Write(size, theBuffer.GetBuffer());
        }

        public bool Read(ByteBuffer theBuffer)
        {
            var size = ReadInt(10);

            theBuffer.Resize(size);

            return Read(size, theBuffer.GetBuffer());
        }

        public bool WriteFlag(bool flag)
        {
            if (BitNum + 1 > MaxWriteBitNum && !ResizeBits(1))
                return false;

            if (flag)
                Data[BitNum >> 3] |= (byte)  (1 << (int) (BitNum & 0x7));
            else
                Data[BitNum >> 3] &= (byte) ~(1 << (int) (BitNum & 0x7));

            ++BitNum;
            return flag;
        }

        public bool ReadFlag()
        {
            if (BitNum > MaxReadBitNum)
            {
                Error = true;
                return false;
            }

            var mask = 1 << ((int) BitNum & 0x7);
            var ret = (Data[BitNum >> 3] & mask) != 0;

            ++BitNum;

            return ret;
        }

        public bool Write(bool value)
        {
            WriteFlag(value);
            return !Error;
        }

        public bool Read(out bool value)
        {
            value = ReadFlag();
            return !Error;
        }

        #region Strings

        public void WriteString(string text, byte maxLen = 255)
        {
            if (text == null)
                text = "";

            var chars = GetCharsForText(text, maxLen);

            byte j;
            for (j = 0; j < maxLen && StringBuffer[j] == chars[j] && StringBuffer[j] != 0; ++j) { }

            Array.Copy(chars, j, StringBuffer, j, maxLen - j);
            StringBuffer[maxLen] = 0;

            if (WriteFlag(j > 2))
            {
                WriteInt(j, 8);
                WriteHuffBuffer(j, (byte) (maxLen - j));
            }
            else
                WriteHuffBuffer(0, maxLen);
        }

        private static byte[] GetCharsForText(string text, byte maxlen)
        {
            var ret = new byte[maxlen];
            var txtb = Encoding.UTF8.GetBytes(text);

            Array.Copy(txtb, 0, ret, 0, Math.Min(maxlen, text.Length));

            return ret;
        }

        public void ReadString(out string stringBuf)
        {
            ReadHuffBuffer(out stringBuf, (byte)(ReadFlag() ? ReadInt(8) : 0));
        }

        private void ReadHuffBuffer(out string stringBuffer, byte off = 0)
        {
            HuffmanTree.Build();

            uint len;

            if (ReadFlag())
            {
                len = ReadInt(8);

                for (var i = 0; i < len; ++i)
                {
                    var current = HuffmanTree.Root;

                    while (true)
                    {
                        if (!HuffmanTree.IsLeaf(current))
                        {
                            current = ReadFlag() ? current.Right : current.Left;
                            continue;
                        }

                        StringBuffer[i + off] = current.Symbol;
                        break;
                    }
                }
            }
            else
            {
                len = ReadInt(8);

                var buff = new byte[len];

                Read(len, buff);

                Array.Copy(buff, 0, StringBuffer, off, len);

                StringBuffer[off + len] = 0;
            }

            stringBuffer = Encoding.UTF8.GetString(StringBuffer, 0, (int) (len + off));
        }

        private void WriteHuffBuffer(byte off, byte maxlen)
        {
            if (StringBuffer[off] == 0)
            {
                WriteFlag(false);
                WriteInt(0, 8);
                return;
            }

            HuffmanTree.Build();

            var len = Strlen(StringBuffer, off);
            if (len > maxlen)
                len = maxlen;

            var numBits = 0U;
            for (var i = 0; i < len; ++i)
                numBits += HuffmanTree.Leaves[StringBuffer[off + i]].NumBits;

            var flag = WriteFlag(numBits < (len * 8));
            WriteInt(len, 8);

            if (flag)
            {
                for (var i = 0; i < len; ++i)
                {
                    var leaf = HuffmanTree.Leaves[StringBuffer[off + i]];

                    var code = BitConverter.GetBytes(leaf.Code);

                    WriteBits(leaf.NumBits, code);
                }
            }
            else
            {
                var temp = new byte[len];

                Array.Copy(StringBuffer, off, temp, 0, len);

                Write(len, temp);
            }
        }

        private static uint Strlen(byte[] buffer, byte off)
        {
            var c = 0U;

            while (buffer[off + c] > 0)
                ++c;

            return c;
        }

        #endregion Strings

        public void WriteStringTableEntry(StringTableEntry ste)
        {
            if (StringTable != null)
                StringTable.WriteStringTableEntry(this, ste);
            else
                WriteString(ste.GetString());
        }

        public void ReadStringTableEntry(out StringTableEntry ste)
        {
            if (StringTable != null)
                ste = StringTable.ReadStringTableEntry(this);
            else
            {
                ReadString(out string buf);

                ste = new StringTableEntry();
                ste.Set(buf.Contains("\0") ? buf.Substring(0, buf.IndexOf('\0')) : buf);
            }
        }

        public bool Write(uint numBytes, byte[] buffer)
        {
            return WriteBits(numBytes << 3, buffer);
        }

        public bool Read(uint numBytes, byte[] buffer)
        {
            return ReadBits(numBytes << 3, buffer);
        }

        #region TemplatizedReadWrite

        public bool Write(byte value)
        {
            var temp = value;

            for (var i = 0; i < 1; ++i)
            {
                CurrentByte[0] = (byte) ((temp >> (i * 8)) & 0xFF);

                if (i != 1 - 1)
                    WriteBits(8, CurrentByte);
            }

            return WriteBits(8, CurrentByte);
        }

        public bool Read(out byte value)
        {
            var temp = new byte[1];

            var success = Read(1, temp);

            value = temp[0];

            return success;
        }

        public bool Write(sbyte value)
        {
            return Write(1, new[] { (byte) value });
        }

        public bool Read(out sbyte value)
        {
            var arr = new byte[1];

            var success = Read(1, arr);

            value = (sbyte) arr[0];

            return success;
        }

        public bool Write(ushort value)
        {
            return Write(2, BitConverter.GetBytes(value));
        }

        public bool Read(out ushort value)
        {
            var arr = new byte[2];

            var success = Read(2, arr);

            value = BitConverter.ToUInt16(arr, 0);

            return success;
        }

        public bool Write(short value)
        {
            return Write(2, BitConverter.GetBytes(value));
        }

        public bool Read(out short value)
        {
            var arr = new byte[2];

            var success = Read(2, arr);

            value = BitConverter.ToInt16(arr, 0);

            return success;
        }

        public bool Write(uint value)
        {
            return Write(4, BitConverter.GetBytes(value));
        }

        public bool Read(out uint value)
        {
            var arr = new byte[4];

            var success = Read(4, arr);

            value = BitConverter.ToUInt32(arr, 0);

            return success;
        }

        public bool Write(int value)
        {
            return Write(4, BitConverter.GetBytes(value));
        }

        public bool Read(out int value)
        {
            var arr = new byte[4];

            var success = Read(4, arr);

            value = BitConverter.ToInt32(arr, 0);

            return success;
        }

        public bool Write(ulong value)
        {
            return Write(8, BitConverter.GetBytes(value));
        }

        public bool Read(out ulong value)
        {
            var arr = new byte[8];

            var success = Read(8, arr);

            value = BitConverter.ToUInt64(arr, 0);

            return success;
        }

        public bool Write(long value)
        {
            return Write(8, BitConverter.GetBytes(value));
        }

        public bool Read(out long value)
        {
            var arr = new byte[8];

            var success = Read(8, arr);

            value = BitConverter.ToInt64(arr, 0);

            return success;
        }

        public bool Write(float value)
        {
            return Write(4, BitConverter.GetBytes(value));
        }

        public bool Read(out float value)
        {
            var arr = new byte[4];

            var success = Read(4, arr);

            value = BitConverter.ToSingle(arr, 0);

            return success;
        }

        public bool Write(double value)
        {
            return Write(8, BitConverter.GetBytes(value));
        }

        public bool Read(out double value)
        {
            var arr = new byte[8];

            var success = Read(8, arr);

            value = BitConverter.ToDouble(arr, 0);

            return success;
        }

        #endregion TemplatizedReadWrite

        public bool SetBit(uint bitCount, bool set)
        {
            if (bitCount >= MaxWriteBitNum && !ResizeBits(bitCount - MaxWriteBitNum + 1))
                return false;

            if (set)
                GetBuffer()[bitCount >> 3] |= (byte)  (1 << ((int) bitCount & 0x7));
            else
                GetBuffer()[bitCount >> 3] &= (byte) ~(1 << ((int) bitCount & 0x7));

            return true;
        }

        public bool TestBit(uint bitCount)
        {
            return (GetBuffer()[bitCount >> 3] & (1 << ((int) bitCount & 0x7))) != 0;
        }

        public bool IsFull()
        {
            return BitNum > (GetBufferSize() << 3);
        }

        public bool IsValid()
        {
            return !Error;
        }

        public void HashAndEncrypt(uint hashDigestSize, uint encryptStartOffset, SymmetricCipher theCipher)
        {
            var digestStart = GetBytePosition();
            SetBytePosition(digestStart);

            var hash = new SHA256Managed().ComputeHash(Data, 0, (int) digestStart);

            Write(hashDigestSize, hash);

            theCipher.Encrypt(GetBuffer(), encryptStartOffset, GetBuffer(), encryptStartOffset, GetBytePosition() - encryptStartOffset);
        }

        public bool DecryptAndCheckHash(uint hashDigestSize, uint decryptStartOffset, SymmetricCipher theCipher)
        {
            var bufferSize = GetBufferSize();
            var buffer = GetBuffer();

            if (bufferSize < decryptStartOffset + hashDigestSize)
                return false;

            theCipher.Decrypt(buffer, decryptStartOffset, buffer, decryptStartOffset, bufferSize - decryptStartOffset);

            var hash = new SHA256Managed().ComputeHash(buffer, 0, (int) (bufferSize - hashDigestSize));

            var ret = Memcmp(buffer, bufferSize - hashDigestSize, hash, 0U, hashDigestSize);
            if (ret)
                Resize(bufferSize - hashDigestSize);

            return ret;
        }

        private static bool Memcmp(byte[] a, uint offsetA, byte[] b, uint offsetB, uint length)
        {
            for (var i = 0U; i < length; ++i)
                if (a[offsetA + i] != b[offsetB + i])
                    return false;

            return true;
        }
    }
}
