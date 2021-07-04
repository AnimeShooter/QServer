using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Qserver.GameServer.Singleton;
using Qserver.GameServer.Network;
using Qserver.GameServer.Network.Packets;
using Qserver.GameServer.Qpang;

namespace Qserver.GameServer.Network
{
    public class SquareManager : SingletonBase<SquareManager>
    {
        //public PacketWriter Login()
        //{
        //    PacketWriter pw = new PacketWriter(Opcode.SQUARE_LOGIN, 0x05);
           
        //    return pw;
        //}

        public PacketWriter SquareList(List<Square> squares)
        {
            PacketWriter pw = new PacketWriter(Opcode.SQUARE_LOGIN_RSP);
            ushort len = (ushort)squares.Count;

            pw.WriteUInt16(len);
            pw.WriteUInt16(len);
            pw.WriteUInt16(len);
            foreach(var square in squares)
            {
                // square
                pw.WriteBytes(new byte[5]);
                pw.WriteUInt32(square.Id);
                pw.WriteUInt8(square.Capacity);
                pw.WriteUInt8(square.PlayerCount);
                pw.WriteUInt8(square.State);
                pw.WriteWString(square.Name, 16);
                pw.WriteBytes(new byte[33]);
            }

            return pw;
        }

        public PacketWriter UpdateSquareEntry(Square square, bool isUpdated = true)
        {
            PacketWriter pw = new PacketWriter(Opcode.SQUARE_UPDATE_LIST);
            pw.WriteInt32(isUpdated ? 0 : 1);

            // square
            pw.WriteBytes(new byte[5]);
            pw.WriteUInt32(square.Id);
            pw.WriteUInt8(square.Capacity);
            pw.WriteUInt8(square.PlayerCount);
            pw.WriteUInt8(square.State);
            pw.WriteWString(square.Name, 16);
            pw.WriteBytes(new byte[33]);

            return pw;
        }

        public PacketWriter JoinSquareSuccess(SquarePlayer squarePlayer)
        {
            PacketWriter pw = new PacketWriter(Opcode.SQUARE_JOIN_PARK_RSP);

            var square = squarePlayer.Square;

            pw.WriteUInt16((ushort)square.Id);
            pw.WriteBytes(new byte[7]);
            pw.WriteUInt32(square.Id);
            pw.WriteUInt8(square.Capacity);
            pw.WriteUInt8(square.PlayerCount);
            pw.WriteUInt8(square.State);
            pw.WriteWString(square.Name, 16);
            pw.WriteBytes(new byte[33]);

            pw.WriteFloat(0f); // squarePlayer.Position.X)
            pw.WriteFloat(0f); //squarePlayer.Position.Y)
            pw.WriteFloat(0f); //squarePlayer.Position.Z)

            return pw;
        }
    }
}
