using System;
using System.IO;
using System.Text;
//using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using Qserver.GameServer.Helpers;
using Qserver.GameServer;

namespace Qserver.GameServer.Network.Packets
{
    public class PacketWriter : BinaryWriter
    {
        private static readonly byte[] PublicKey = new byte[] { 0x66, 0x64, 0x24, 0x23, 0x32, 0x3E, 0x34, 0x35, 0x7D, 0x5F, 0x7E, 0x2E, 0x33, 0x38, 0x4C, 0x61, 0x60, 0x27, 0x2B, 0x52, 0x45, 0x2F, 0x25, 0x2D, 0x49, 0x61, 0x3D, 0x7C, 0x39, 0x58, 0x28, 0x3F, 0x00 };

        public Opcode Opcode { get; set; }
        public ushort Size { get; set; }
        public int Length { get { return (int)BaseStream.Length; } }


        public PacketWriter() : base(new MemoryStream()) { }
        public PacketWriter(Opcode opcode) : base(new MemoryStream())
        {
            Opcode = opcode;
            WritePacketHeader();
        }

        protected void WritePacketHeader()
        {

            WriteUInt8((byte)Opcode);
        }

        public byte[] ReadDataToSend(bool isAuthPacket = false)
        {
            // TODO: WriteChecksum();

            byte[] data = new byte[BaseStream.Length];
            Seek(0, SeekOrigin.Begin);

            Size = (ushort)(data.Length);

            for (int i = 0; i < BaseStream.Length; i++)
                data[i] = (byte)BaseStream.ReadByte();


            return data;
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

        public void WriteBytes(byte[] data)
        {
            base.Write(data);
        }

        public void WriteVector(Vector data)
        {
            base.Write(data.X);
            base.Write(data.Y);
        }

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
