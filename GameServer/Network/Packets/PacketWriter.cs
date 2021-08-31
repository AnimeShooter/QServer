using System;
using System.IO;
using System.Text;
using Qserver.GameServer;
using System.Linq;
using System.Collections.Generic;
using Qserver.Util;

namespace Qserver.GameServer.Network.Packets
{
    public class PacketWriter : BinaryWriter
    {
        private static readonly byte[] PublicKey = new byte[] { 0x66, 0x64, 0x24, 0x23, 0x32, 0x3E, 0x34, 0x35, 0x7D, 0x5F, 0x7E, 0x2E, 0x33, 0x38, 0x4C, 0x61, 0x60, 0x27, 0x2B, 0x52, 0x45, 0x2F, 0x25, 0x2D, 0x49, 0x61, 0x3D, 0x7C, 0x39, 0x58, 0x28, 0x3F, 0x00 };

        public Opcode Opcode { get; set; }
        public ushort RawSize
        {
            get { return (ushort)(BaseStream.Length + 12); }
        }
        public ushort Size
        {
            get { return (ushort)(RawSize - 4); }
        }

        public byte Encryption;

        public PacketWriter() : base(new MemoryStream()) { }
        public PacketWriter(Opcode opcode) : base(new MemoryStream())
        {
            Opcode = opcode;
            Encryption = 0x03; // public idk
        }

        public PacketWriter(Opcode opcode, byte encryption) : base(new MemoryStream())
        {
            Opcode = opcode;
            Encryption = encryption; // 0x05 for ServerPacket
        }

        public byte[] ReadDataToSend(byte[] key = null)
        {
            // TODO: WriteChecksum();
            // TODO: Cleanup
            // TODO: Fix encryption!

            // Header
            byte[] header = new byte[4];
            byte[] rawsizebytes = BitConverter.GetBytes((UInt16)RawSize);
            header[0] = rawsizebytes[0];
            header[1] = rawsizebytes[1];
            header[2] = 0x00; // counter?
            header[3] = 69; // crc ?

            byte[] payload = new byte[Size];

            // Payload Header
            byte[] sizebytes = BitConverter.GetBytes((UInt16)Size);
            byte[] opcodeBytes = BitConverter.GetBytes((UInt16)Opcode);
            payload[0] = sizebytes[0];
            payload[1] = sizebytes[1];
            // 
            payload[2] = opcodeBytes[0];
            payload[3] = opcodeBytes[1];
            // unk
            //payload[4] = 0x42;// opcodeBytes[1];
            //payload[5] = 0x42;//opcodeBytes[1];
            //payload[6] = 0x42;//opcodeBytes[1];
            //payload[7] = 0x42;//opcodeBytes[1];
            Seek(0, SeekOrigin.Begin);

            for (int i = 8; i < Size; i++)
                payload[i] = (byte)BaseStream.ReadByte();

            BlowFish b = BlowFish.Instance;
            if (Encryption >= 0x05 && key != null && Opcode != Opcode.KEY_EXCHANGE_RSP) // dont use key when first handing out key
            {
                b = new BlowFish(key);
                b.CompatMode = true;
                //header[2] = 0x05; // auth
            }

            var final = new List<byte>();
            final.AddRange(header);

            // encrypted
            //final.AddRange(b.Encrypt_ECB(payload));
            //final[0] = (byte)final.Count;

            // plaintext
            final.AddRange(payload);

            return final.ToArray();
        }

        public void Seek(int offset)
        {
            base.Seek(offset, SeekOrigin.Begin);
        }

        public void WriteInt8(sbyte data)
        {
            base.Write(data);
        }

        public void WriteInt16(short data)
        {
            base.Write(data);
        }

        public void WriteInt32(int data)
        {
            base.Write(data);
        }

        public void WriteInt64(long data)
        {
            base.Write(data);
        }

        public void WriteUInt8(byte data)
        {
            base.Write(data);
        }

        public void WriteUInt16(ushort data)
        {
            base.Write(data);
        }

        public void WriteUInt32(uint data)
        {
            base.Write(data);
        }

        public void WriteUInt64(ulong data)
        {
            base.Write(data);
        }

        public void WriteFloat(float data)
        {
            base.Write(data);
        }

        public void WriteDouble(double data)
        {
            base.Write(data);
        }

        public void WriteString(string data)
        {
            byte[] sBytes = Encoding.ASCII.GetBytes(data);
            this.WriteBytes(sBytes);
            base.Write((byte)0);    // String null terminated
        }

        public void WriteString(string data, int max)
        {
            byte[] sBytes = Encoding.ASCII.GetBytes(data);
            for(int i = 0; i < max; i++)
            {
                if (i < sBytes.Length)
                {
                    //this.WriteUInt8(0);
                    this.WriteUInt8(sBytes[i]);
                } 
                else
                {
                    //this.WriteUInt8(0);
                    this.WriteUInt8(0);
                }
                    
            }
            //base.Write((byte)0);    // String null terminated
        }

        public void WriteWString(string data, int max)
        {
            //char[] sBytes = Encoding.UTF8.GetBytes(data);
            for(int i = 0; i < max; i++)
            {
                if(i < data.Length)
                {
                    byte[] wchar = BitConverter.GetBytes((char)data[i]);
                    this.WriteBytes(wchar);
                }
                else
                    this.WriteUInt16(0);
            }
            base.Write(new byte[2]);    // String null null terminated
        }

        public void WriteBytes(byte[] data)
        {
            base.Write(data);
        }

        //public void WriteVector(Vector data)
        //{
        //    base.Write(data.X);
        //    base.Write(data.Y);
        //}

        public void WriteChecksum()
        {
            byte[] checksumBytes = { 0x9C, 0x14, 0xED, 0x29, 0xF2, 0xB5, 0x83, 0x7A };
            //ushort extra = p

         //   uint16_t extra = (payload.size() + 2) & 7;
         //   if (!extra)
         //   {
         //       return;
         //   }

         //   uint16_t amount = 8 - extra;
         //   //buffer.resize(buffer.size() + amount + 2);
         //   for (uint16_t i{ 0 }; i < amount; ++i)
	        //{
         //       payload.push_back(payload[i] ^ checksumBytes[i]);
         //   }

         //   payload.push_back(static_cast<uint8_t>(amount & 0xFF));
         //   payload.push_back(static_cast<uint8_t>((amount >> 8) & 0xFF));
        }
    }
}
