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

        public PacketWriter AddPlayer(SquarePlayer squarePlayer)
        {
            PacketWriter pw = new PacketWriter(Opcode.SQUARE_ADD_PLAYER);

            // squarePlayer
            pw.WriteUInt32(squarePlayer.State);
            pw.WriteUInt32(squarePlayer.Player.PlayerId);
            pw.WriteWString(squarePlayer.Player.Name, 16);
            pw.WriteUInt8((byte)squarePlayer.Player.Level);
            pw.WriteUInt8((byte)squarePlayer.Player.Rank);
            pw.WriteUInt16(0);
            pw.WriteUInt16(squarePlayer.Player.Character);
            pw.WriteUInt32(0); // TODO: select weapon
            //pw.WriteBytes(squarePlayer.Player.EquipmentManager.)
            pw.WriteBytes(new byte[9 * 4]);
            pw.WriteBytes(new byte[12]);
            pw.WriteFloat(squarePlayer.Position[0]);
            pw.WriteFloat(squarePlayer.Position[1]);
            pw.WriteFloat(squarePlayer.Position[2]);

            return pw;
        }

    }
}
