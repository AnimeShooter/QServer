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

        public enum Commands : uint
        {
            MAP_ROOM = 1, // PvE difficulty
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
        public uint Cmd = (uint)Commands.CREATE_ROOM;
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
            Map = Value; // union

            bitStream.Read(out Mode);
            RoomId = Mode;

            bitStream.Read(out MemberCount);
            RoomId2 = MemberCount; // union

            bitStream.Read(out Goal);
            bitStream.ReadString(out Password);

            ByteBuffer titleBuffer = new ByteBuffer(22); // 256
            bitStream.Read(titleBuffer);
            Title = ByteBufferToString(titleBuffer);

            bitStream.Read(out TimeAmount);
            PointsAmount = TimeAmount;

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

            Console.WriteLine($"{(CGRoom.Commands)Cmd}, Value: {Value}, Mode: {Mode}, MemCount: {MemberCount}, Goal: {Goal}, pw:{Password}, Title: {Title}, TimeAmount: {TimeAmount}, IsRounds: {IsRounds}");
        }
        public override void Process(EventConnection ps)
        {
            Post(ps);
        }
        public override void Handle(GameConnection conn, Player player)
        {

            if (Cmd == (uint)Commands.CREATE_ROOM || Cmd == (uint)Commands.CREATE_EVENT_ROOM)
            {
                if (Cmd == (uint)Commands.CREATE_EVENT_ROOM && player.Rank != 3)
                {
                    conn.Disconnect("Cannot create event room");
                    return;
                }

                bool validMode = Mode == (uint)GameMode.Mode.DM ||
                                 Mode == (uint)GameMode.Mode.TDM ||
                                 Mode == (uint)GameMode.Mode.PTE ||
                                 Mode == (uint)GameMode.Mode.VIP;

                if ((!validMode || Map > 12) && player.Rank < 3) // NOTE: GMs are allowed
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

                var newroom = Game.Instance.RoomManager.Create(Title, (byte)Map, (GameMode.Mode)Mode, Settings.SERVER_IP, (ushort)Settings.SERVER_PORT_ROOM);
                //var newroom = Game.Instance.RoomManager.Create(Title, (byte)Map, (GameMode.Mode)Mode, conn.Ip, conn.Port); // P2P ?
                //Util.Log.Message(Util.LogType.MISC, "New room host at: " + conn.Ip.ToString("X8") + ":" + conn.Port);
                
                newroom.EventRoom = Cmd == (uint)Commands.CREATE_EVENT_ROOM;
                newroom.AddPlayer(conn);
            }
            else if (Cmd == (uint)Commands.JOIN_ROOM)
            {
                var joinroom = Game.Instance.RoomManager.Get(RoomId);

                if (joinroom == null)
                {
                    conn.Disconnect("Could not find selected room");
                    return;
                }

                if (joinroom.PlayerCount >= joinroom.MaxPlayers)
                {
                    conn.Disconnect("Room is full");
                    return;
                }

                if (joinroom.Password != "" && player.Rank < 3)
                    if (joinroom.Password != Password)
                        return;

                joinroom.AddPlayer(conn);
            }
            else
            {
                var roomPlayer = player.RoomPlayer;
                if (roomPlayer == null)
                    return;

                var room = roomPlayer.Room;
                if (room.MasterId != player.PlayerId)
                    return;

                if (room.Playing)
                    return;

                switch ((Commands)Cmd)
                {
                    case Commands.MAP_ROOM:
                        room.SetMap((byte)Value);
                        return;
                    case Commands.MODE_ROOM:
                        bool validMode = Mode == (uint)GameMode.Mode.DM ||
                            Mode == (uint)GameMode.Mode.TDM ||
                            Mode == (uint)GameMode.Mode.PTE ||
                            Mode == (uint)GameMode.Mode.VIP;
                        if ((!validMode || Map > 12) && player.Rank < 3) // NOTE: GMs are allowed
                        {
                            conn.PostNetEvent(new GCRoom(player.PlayerId, (uint)Commands.MODE_ROOM, room));
                            player.Broadcast("Sorry, this game mode has not been implemented yet");
                            return;
                        }
                        room.SetMode((GameMode.Mode)Mode);
                        break;
                    case Commands.SET_POINTS:
                        room.SetScorePoints(Value);
                        room.PointsGame = true;
                        break;
                    case Commands.SET_TIME:
                        room.SetScoreTime(Value);
                        room.PointsGame = false;
                        break;
                    case Commands.PLAYERS_ROOM:
                        if (Value == 4 || Value == 8 || Value == 12 || Value == 16)
                            room.SetMaxPlayers((byte)Value);
                        break;
                    case Commands.PASS_ROOM:
                        room.SetPassword(Password);
                        break;
                    case Commands.LEVEL_ROOM:
                        room.SetLevelLimited(Value == 1 ? true : false);
                        break;
                    case Commands.TOGGLE_MELEE:
                        room.SetMeleeOnly(Value == 1 ? true : false);
                        break;
                    case Commands.TEAM_ROOM: // TODO: fix
                        room.SetTeamSorting(Value == 1 ? true : false);
                        break;
                    case Commands.TOGGLE_SKILL:
                        room.SetSkillsEnabled(Value == 1 ? true : false);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
