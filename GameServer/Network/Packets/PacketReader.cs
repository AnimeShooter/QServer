using Qserver.GameServer.Helpers;
using System;
using System.Linq;
using System.IO;
using System.Text;
using Qserver.Util;

namespace Qserver.GameServer.Network.Packets
{
   
    public struct PacketHeader
    {
        public UInt16 Length;
        public byte Encryption;
        public byte Unk;
    }

    public struct PayloadHeader
    {
        public UInt16 Length;
        public UInt16 Opcode;
    }


    public class PacketReader
    {
        public static readonly byte[] PublicKey = new byte[] { 0x66, 0x64, 0x24, 0x23, 0x32, 0x3E, 0x34, 0x35, 0x7D, 0x5F, 0x7E, 0x2E, 0x33, 0x38, 0x4C, 0x61, 0x60, 0x27, 0x2B, 0x52, 0x45, 0x2F, 0x25, 0x2D, 0x49, 0x61, 0x3D, 0x7C, 0x39, 0x58, 0x28, 0x3F, 0x00 };

        private PacketHeader PacketHeader;
        private PayloadHeader PayloadHeader;

        public Opcode Opcode
        {
            get { return (Opcode)PayloadHeader.Opcode; }
        }
        public ushort Size
        {
            get { return PacketHeader.Length; }
        }
        public ushort RawSize
        {
            get { return PayloadHeader.Length; }
        }
        public bool IsEncrypted
        {
            get { return PacketHeader.Encryption != 0x00; }
        }

        /// Packet::Header (0)
        /// Packet::PayloadHeader (4)
        /// Packet::Payload (8)

        private BinaryReader _payload;

        public PacketReader(byte[] data, string identifier)
        {
            PacketHeader = new PacketHeader()
            {
                Length = BitConverter.ToUInt16(data, 0),
                Encryption = data[2],
                Unk = data[3]
            };
            if(IsEncrypted)
            {
                BlowFish b = new BlowFish(PublicKey);
                b.CompatMode = true;
                byte[] encryptedPayload = data.Skip(4).Take(PacketHeader.Length).ToArray();
                byte[] decryptedPayload = b.Decrypt_ECB(encryptedPayload);
                _payload = new BinaryReader(new MemoryStream(decryptedPayload));
            }
            else
                _payload = new BinaryReader(new MemoryStream(this.ReadBytes(PacketHeader.Length)));

            PayloadHeader = new PayloadHeader()
            {
                Length = this.ReadUInt16(),
                Opcode = this.ReadUInt16()
            };
        }

        public sbyte ReadInt8()
        {
            return _payload.ReadSByte();
        }

        public new short ReadInt16()
        {
            return _payload.ReadInt16();
        }

        public new int ReadInt32()
        {
            return _payload.ReadInt32();
        }

        public new long ReadInt64()
        {
            return _payload.ReadInt64();
        }

        public byte ReadUInt8()
        {
            return _payload.ReadByte();
        }

        public new ushort ReadUInt16()
        {
            return _payload.ReadUInt16();
        }

        public new uint ReadUInt32()
        {
            return _payload.ReadUInt32();
        }

        public new ulong ReadUInt64()
        {
            return _payload.ReadUInt64();
        }

        public float ReadFloat()
        {
            return _payload.ReadSingle();
        }

        public new double ReadDouble()
        {
            return _payload.ReadDouble();
        }

        public Vector ReadVector()
        {
            return new Vector(_payload.ReadUInt16(), _payload.ReadUInt16());
        }

        public string ReadString(byte terminator = 0)
        {
            StringBuilder tmpString = new StringBuilder();
            char tmpChar = _payload.ReadChar();
            char tmpEndChar = Convert.ToChar(Encoding.UTF8.GetString(new byte[] { terminator }));

            while (tmpChar != tmpEndChar)
            {
                tmpString.Append(tmpChar);
                tmpChar = _payload.ReadChar();
            }

            return tmpString.ToString();
        }

        public new string ReadString()
        {
            return ReadString(0);
        }

        public new byte[] ReadBytes(int count)
        {
            return _payload.ReadBytes(count);
        }

        public string ReadStringFromBytes(int count)
        {
            byte[] stringArray = _payload.ReadBytes(count);
            Array.Reverse(stringArray);

            return Encoding.ASCII.GetString(stringArray);
        }

        public string ReadIPAddress()
        {
            byte[] ip = new byte[4];

            for (int i = 0; i < 4; ++i)
            {
                ip[i] = ReadUInt8();
            }

            return ip[0] + "." + ip[1] + "." + ip[2] + "." + ip[3];
        }

        public void SkipBytes(int count)
        {
            _payload.BaseStream.Position += count;
        }
    }
}
