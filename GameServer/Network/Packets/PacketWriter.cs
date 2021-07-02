﻿using System;
using System.IO;
using System.Text;
//using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using Qserver.GameServer.Helpers;
using Qserver.GameServer;

namespace Qserver.GameServer.Network.Packets
{
    public class PacketWriter : BinaryWriter
    {
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

        
        //public void Compress()
        //{
        //    if (Opcode != Opcodes.SMSG_UPDATE_OBJECT || BaseStream.Length <= 100)
        //        return;

        //    byte[] outBuffer = null;
        //    int baseSize = (int)BaseStream.Length - 6; //Minus header size

        //    try
        //    {
        //        using (MemoryStream outputStream = new MemoryStream())
        //        {
        //            using (DeflaterOutputStream compressedStream = new DeflaterOutputStream(outputStream))
        //            {
        //                byte[] data = ReadDataToSend();
        //                compressedStream.Write(data, 6, data.Length - 6); //Get the packet data 
        //                compressedStream.Flush();
        //                compressedStream.Close();
        //                outBuffer = outputStream.ToArray();
        //            }
        //        }
        //    }
        //    catch { return; }

        //    if (outBuffer.Length >= baseSize)
        //        return;

        //    BaseStream.Seek(0, SeekOrigin.Begin); //Reset the packet
        //    BaseStream.SetLength(0);

        //    Opcode = Opcodes.SMSG_COMPRESSED_UPDATE_OBJECT; //Create the compressed packet
        //    WritePacketHeader();
        //    WriteInt32(baseSize);
        //    WriteBytes(outBuffer);
        //}
    }
}
