using Qserver.GameServer.Helpers;
using System;
using System.IO;
using System.Text;

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


    public class PacketReader : BinaryReader
    {

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
            get { return PacketHeader.Encryption == 0x01; }
        }

        /// Packet::Header (0)
        /// Packet::PayloadHeader (4)
        /// Packet::Payload (8)


        public PacketReader(byte[] data, string identifier) : base(new MemoryStream(data))
        {
            PacketHeader = new PacketHeader()
            {
                Length = this.ReadUInt16(),
                Encryption = this.ReadByte(),
                Unk = this.ReadByte()
            };
            if(IsEncrypted)
            {
                // TODO Blow sum fish
            }
            else
            {
                PayloadHeader = new PayloadHeader()
                {
                    Length = this.ReadUInt16(),
                    Opcode = this.ReadUInt16()
                };
            }
        }

        public sbyte ReadInt8()
        {
            return base.ReadSByte();
        }

        public new short ReadInt16()
        {
            return base.ReadInt16();
        }

        public new int ReadInt32()
        {
            return base.ReadInt32();
        }

        public new long ReadInt64()
        {
            return base.ReadInt64();
        }

        public byte ReadUInt8()
        {
            return base.ReadByte();
        }

        public new ushort ReadUInt16()
        {
            return base.ReadUInt16();
        }

        public new uint ReadUInt32()
        {
            return base.ReadUInt32();
        }

        public new ulong ReadUInt64()
        {
            return base.ReadUInt64();
        }

        public float ReadFloat()
        {
            return base.ReadSingle();
        }

        public new double ReadDouble()
        {
            return base.ReadDouble();
        }

        public Vector ReadVector()
        {
            return new Vector(base.ReadUInt16(), base.ReadUInt16());
        }

        public string ReadString(byte terminator = 0)
        {
            StringBuilder tmpString = new StringBuilder();
            char tmpChar = base.ReadChar();
            char tmpEndChar = Convert.ToChar(Encoding.UTF8.GetString(new byte[] { terminator }));

            while (tmpChar != tmpEndChar)
            {
                tmpString.Append(tmpChar);
                tmpChar = base.ReadChar();
            }

            return tmpString.ToString();
        }

        public new string ReadString()
        {
            return ReadString(0);
        }

        public new byte[] ReadBytes(int count)
        {
            return base.ReadBytes(count);
        }

        public string ReadStringFromBytes(int count)
        {
            byte[] stringArray = base.ReadBytes(count);
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
            base.BaseStream.Position += count;
        }
    }
}
