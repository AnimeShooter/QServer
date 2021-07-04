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
        public byte Checksum;
    }

    public struct PayloadHeader
    {
        public UInt16 Length;
        public UInt16 Opcode;
    }


    public class PacketReader
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
        public byte Encryption
        {
            get { return PacketHeader.Encryption; }
        }

        /// Packet::Header (0)
        /// Packet::PayloadHeader (4)
        /// Packet::Payload (8)

        private BinaryReader _payload;

        public PacketReader(byte[] data, string identifier, byte[] key)
        {
            PacketHeader = new PacketHeader()
            {
                Length = BitConverter.ToUInt16(data, 0),
                Encryption = data[2],
                Checksum = data[3]
            };
            if(Encryption > 0)
            {
                BlowFish b = BlowFish.Instance;
                if (Encryption >= 0x05)
                {
                    b = new BlowFish(key); // xx xx xx xx 29 A1 D3 56
                    b.CompatMode = true;
                    //b = new BlowFish(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x29, 0xA1, 0xD3, 0x56 }); // 29 A1 D3 56 29 A1 D3 56

                }

                byte[] encryptedPayload = data.Skip(4).Take(PacketHeader.Length).ToArray();
                byte[] decryptedPayload = b.Decrypt_ECB(encryptedPayload);
                _payload = new BinaryReader(new MemoryStream(decryptedPayload));
            }
            else
                _payload = new BinaryReader(new MemoryStream(data.Skip(4).Take(PacketHeader.Length).ToArray()));

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
