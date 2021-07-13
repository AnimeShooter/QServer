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
                //pw.WriteBytes(new byte[16] { 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, });

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
            //pw.WriteBytes(new byte[16] { 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, });

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
            //pw.WriteBytes(new byte[16] { 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, });

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
            //pw.WriteBytes(new byte[16] { 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, });

            pw.WriteUInt8((byte)squarePlayer.Player.Level);
            pw.WriteUInt8((byte)squarePlayer.Player.Rank);
            pw.WriteUInt16(0);
            pw.WriteUInt16(squarePlayer.Player.Character);
            pw.WriteUInt32(0); // TODO: select weapon

            foreach(var armor in squarePlayer.Player.EquipmentManager.GetArmorItemIdsByCharacter(squarePlayer.Player.Character))
                pw.WriteUInt32(armor); // 9

            pw.WriteFloat(squarePlayer.Position[0]);
            pw.WriteFloat(squarePlayer.Position[1]);
            pw.WriteFloat(squarePlayer.Position[2]);

            return pw;
        }

        public PacketWriter Chat(string sender, string message)
        {
            PacketWriter pw = new PacketWriter((Opcode)6529);

            pw.WriteWString(sender, 16);
            //pw.WriteBytes(new byte[16] { 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, });

            ushort len = (ushort)(message.Length % 254);
            pw.WriteUInt16(len);
            pw.WriteWString(message, len);
            return pw;
        }

        public PacketWriter DeleteSquareEntry(uint id)
        {
            PacketWriter pw = new PacketWriter((Opcode)6543);
            pw.WriteUInt32(1);
            pw.WriteBytes(new byte[5]);
            pw.WriteUInt32(id);
            return pw;
        }

        public PacketWriter Emote(uint playerId, uint emoteId)
        {
            PacketWriter pw = new PacketWriter((Opcode)6558);
            pw.WriteUInt32(playerId);
            pw.WriteUInt32(emoteId);
            return pw;
        }

        public PacketWriter MovePlayer(uint playerId, float[] newPosition, byte moveType, byte direction)
        {
            PacketWriter pw = new PacketWriter((Opcode)6513);
            pw.WriteUInt32(playerId);
            pw.WriteUInt8(moveType);
            pw.WriteUInt8(direction);
            pw.WriteFloat(newPosition[0]);
            pw.WriteFloat(newPosition[1]);
            pw.WriteFloat(newPosition[2]);
            return pw;
        }

        public PacketWriter Players(List<SquarePlayer> squarePlayers, uint playerId)
        {
            //return;
            PacketWriter pw = new PacketWriter(Opcode.SQUARE_LOAD_PLAYERS);

            ushort len = (ushort)squarePlayers.Count;
            //ushort len = 0; // (ushort)squarePlayers.Count;

            pw.WriteUInt16(len);
            pw.WriteUInt16(len);
            pw.WriteUInt16(len);

            for(int i = 0; i < 100; i++)
            //for(int i = 0; i < len; i++)
            {
                if (i < len)
                {
                    // squarePlayer
                    pw.WriteUInt32(squarePlayers[i].State);             // 4
                    pw.WriteUInt32(squarePlayers[i].Player.PlayerId);   // 8

                    //pw.WriteBytes(new byte[16] { 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, });
                    //pw.WriteBytes(new byte[16] { 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, 0x41, 0x00, 0x42, 0x00, }); // idk?
                    pw.WriteWString(squarePlayers[i].Player.Name, 16);  // 40

                    pw.WriteUInt8((byte)squarePlayers[i].Player.Level); // 41
                    pw.WriteUInt8((byte)squarePlayers[i].Player.Rank);  // 42
                    pw.WriteUInt16(0);                                  // 44
                    pw.WriteUInt16(squarePlayers[i].Player.Character);  // 46
                    pw.WriteUInt32(0); // TODO: select weapon           // 50
                    //pw.WriteBytes(squarePlayer.Player.EquipmentManager.)
                    pw.WriteBytes(new byte[9 * 4]);                     // 86
                    pw.WriteBytes(new byte[2]);                         // 88
                    pw.WriteFloat(squarePlayers[i].Position[0]);        // 92
                    pw.WriteFloat(squarePlayers[i].Position[1]);        // 96
                    pw.WriteFloat(squarePlayers[i].Position[2]);        // 100
                }
                else
                    pw.WriteBytes(new byte[100]);
            }
            pw.WriteUInt32(0);

            return pw;
        }

        public PacketWriter RemovePlayer(ushort playerId)
        {
            PacketWriter pw = new PacketWriter((Opcode)6509);
            pw.WriteUInt32(playerId);
            return pw;
        }

        public PacketWriter SetPosition(SquarePlayer squarePlayer)
        {
            PacketWriter pw = new PacketWriter((Opcode)6531);
            pw.WriteFloat(squarePlayer.Position[0]);
            pw.WriteFloat(squarePlayer.Position[1]);
            pw.WriteFloat(squarePlayer.Position[2]);
            return pw;
        }
        public PacketWriter UpdatePlayerEquipment(SquarePlayer squarePlayer)
        {
            PacketWriter pw = new PacketWriter((Opcode)6517);
            var player = squarePlayer.Player;
            pw.WriteUInt32(player.PlayerId);
            pw.WriteUInt16(player.Character);

            //pw.WriteUInt32(squarePlayer.Selec) // SelectedWeapon
            pw.WriteUInt32(0);

            // selected Character Armor
            for(int i = 0; i < 9; i++)
            {
                pw.WriteUInt32(0);
            }

            return pw;
        }
        public PacketWriter UpdatePlayerLevel(uint playerId, byte level)
        {
            PacketWriter pw = new PacketWriter((Opcode)6553);
            pw.WriteUInt32(playerId);
            pw.WriteUInt8(level);
            return pw;
        }
        public PacketWriter UpdatePlayerState(SquarePlayer squarePlayer, byte value)
        {
            PacketWriter pw = new PacketWriter((Opcode)6547);
            pw.WriteUInt32(squarePlayer.Player.PlayerId);
            pw.WriteUInt32(squarePlayer.State);
            pw.WriteUInt8(value);
            pw.WriteUInt8(0);
            return pw;
        }

    }
}
