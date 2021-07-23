using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNL.Entities;
using TNL.Types;
using TNL.Utils;
using TNL.Network;
using TNL.Interfaces;
using TNL.Data;

namespace Qserver.GameServer.Qpang
{
    public class CGRoom : GameNetEvent
    {
        private static NetClassRepInstance<CGRoom> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CGRoom", NetClassMask.NetClassGroupGameMask, 0);
        }

        public enum Command : uint
        {
            MAP_ROOM = 1,
            PLAYERS_ROOM = 2,
            MODE_ROOM = 3,
            SET_POINTS = 4,
            PASS_ROOM = 5,
            CREATE_ROOM = 6,
            JOIN_ROOM = 7,
            SET_TIME = 20,
            LEVEL_ROOM = 23,
            TEAM_ROOM = 25,
            TOGGLE_SKILL = 26,
            CREATE_EVENT_ROOM = 27,
            TOGGLE_MELEE = 30,
        };

        public uint PlayerId = 0; // 92;
        public uint Cmd = (uint)Command.CREATE_ROOM;
        public uint Value = 0;
        public uint Map;
        public byte Mode;
        public byte RoomId;
        public byte MemberCount = 0;
        public byte RoomId2;
        public byte Goal = 0;
        public string Password = "";
        public string Title = "";
        public byte TimeAmount;
        public byte PointsAmount;
        public byte IsRounds = 0;
        
        public uint unk11;
        public uint unk12;
        public ushort unk13;
        public uint unk14;

        public byte unk_160;
        public byte unk_161;
        public byte unk_162;
        public byte unk_163;

        public uint unk19;
        public uint unk20;
        public uint unk21;
        public uint unk22;
        

        public CGRoom() : base(GameNetId.CG_ROOM, GuaranteeType.GuaranteedOrdered, EventDirection.DirClientToServer)
        {

        }

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Read(out PlayerId);
            bitStream.Read(out Cmd);
            bitStream.Read(out Value);
            bitStream.Read(out Map);
            bitStream.Read(out Mode);
            bitStream.Read(out RoomId);
            bitStream.Read(out MemberCount);
            bitStream.Read(out RoomId2);
            bitStream.Read(out Goal);
            bitStream.ReadString(out Password);
            bitStream.ReadString(out Title);
            bitStream.Read(out TimeAmount);
            bitStream.Read(out PointsAmount);
            bitStream.Read(out IsRounds);
            bitStream.Read(out unk11);
            bitStream.Read(out unk12);
            bitStream.Read(out unk13);
            bitStream.Read(out unk14);
            bitStream.Read(out unk_160);
            bitStream.Read(out unk_161);
            bitStream.Read(out unk_162);
            bitStream.Read(out unk_163);
            bitStream.Read(out unk19);
            bitStream.Read(out unk20);
            bitStream.Read(out unk21);
            bitStream.Read(out unk22);
        }
        public override void Process(EventConnection ps)
        {
            Post(ps);
        }
        public override void Handle(GameConnection conn, Player player)
        {
            switch((Command)Cmd)
            {
                case Command.CREATE_ROOM:
                case Command.CREATE_EVENT_ROOM:
                    if (Cmd == (uint)Command.CREATE_EVENT_ROOM && player.Rank != 3)
                    {
                        conn.Disconnect("Cannot create event room");
                        return;
                    }

                    bool isValidMode = Mode == (uint)GameMode.Mode.DM || Mode == (uint)GameMode.Mode.TDM || Mode == (uint)GameMode.Mode.PTE || Mode == (uint)GameMode.Mode.VIP;
                    if (!isValidMode || Map > 12)
                    {
                        conn.Disconnect("Invalid GameMode");
                        player.Broadcast("GameMode has not been implemented yet");
                        return;
                    }

                    if (Game.Instance.RoomManager.List().Count >= 50) // NOTE: dont harcode?
                    {
                        conn.Disconnect("Failed creating room");
                        return;
                    }

                    var room = Game.Instance.RoomManager.Create(Title, (byte)Map, (GameMode.Mode)Mode);
                    room.EventRoom = Cmd == (uint)Command.CREATE_EVENT_ROOM;
                    room.AddPlayer(conn);
                    break;
                case Command.JOIN_ROOM:
                    var gameroom = Game.Instance.RoomManager.Get(RoomId);

                    if (gameroom == null)
                    {
                        conn.Disconnect("Could not find selected room");
                        return;
                    }

                    if(gameroom.PlayerCount >= gameroom.MaxPlayers)
                    {
                        conn.Disconnect("Room is full");
                        return;
                    }

                    if (gameroom.Password != "" && player.Rank < 3)
                        if (gameroom.Password != Password)
                            return;

                    gameroom.AddPlayer(conn);
                       
                    break;
                default:

                    break;
            }

        }

    }
    
}
