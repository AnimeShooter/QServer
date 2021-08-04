using System;
using System.Collections.Generic;
using System.Text;
using TNL.Data;
using TNL.Entities;
using TNL.Types;
using TNL.Utils;

namespace Qserver.GameServer.Qpang
{
    public class GCRoom : GameNetEvent
    {
        private static NetClassRepInstance<GCRoom> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "GCRoom", NetClassMask.NetClassGroupGameMask, 0);
        }

        private uint PlayerId; // 92
        private uint Cmd; // 96

        // union
        public uint Value;
        public uint RoomId;

        public byte Mode;
        public byte MemberCount;
        public byte Goal = 10;

        public string Password = "";
        public string Title = "";
        public byte Time = 6;

        // union
        public byte Rounds = 1;
        public byte GameId;

        public byte Unk160 = 1; // in CGRoom

        public uint Zero = 0;
        public byte Unk161 = 0;
        public byte SkillsEnabled = 0;
        public byte MeleeOnly = 0;  // 166


        public GCRoom() : base(GameNetId.GC_ROOM, GuaranteeType.Guaranteed, EventDirection.DirAny)
        {
            
        }

        public GCRoom(uint playerId, uint cmd, Room room) : base(GameNetId.GC_ROOM, GuaranteeType.Guaranteed, EventDirection.DirAny)
        {
            // NOTE: 150 error packet
            Zero = 0;
            PlayerId = playerId;
            Cmd = cmd;

            // union
            RoomId = room.Id;
            Value = RoomId;

            Mode = (byte)room.Mode;
            MemberCount = room.PlayerCount;
            Title = room.Name;
            MeleeOnly = room.MeleeOnly ? (byte)1 : (byte)0;
            SkillsEnabled = room.SkillsEnabled ? (byte)1 : (byte)0;
        }
        public GCRoom(uint playerId, uint cmd, uint val, Room room) : base(GameNetId.GC_ROOM, GuaranteeType.Guaranteed, EventDirection.DirAny)
        {
            Zero = 0;
            PlayerId = playerId;
            Cmd = cmd;

            // union
            Value = val;
            RoomId = Value;

            Mode = (byte)room.Mode;
            Title = "-";
            Password = room.Password;
            MeleeOnly = room.MeleeOnly ? (byte)1 : (byte)0;
            SkillsEnabled = room.SkillsEnabled ? (byte)1 : (byte)0;
        }

        public override void Pack(EventConnection conn, BitStream bitStream)
        {
            // NOTE: packing is fucked up (title incorrect)
            bitStream.Write(PlayerId);
            bitStream.Write(Cmd);
            bitStream.Write(Value);

            bitStream.Write(Mode);
            bitStream.Write(MemberCount);
            bitStream.Write(Goal);

            //bitStream.Write((byte)0);  // NoPw
            bitStream.WriteString(Password, 255);

            //bitStream.Write((ushort)0); // NoTitle
            WriteWString(bitStream, Title, 254);

            bitStream.Write(Time);
            bitStream.Write(Rounds); // untion gameId
            bitStream.Write(Unk160);
            bitStream.Write(Zero);
            bitStream.Write(Unk161);
            bitStream.Write(SkillsEnabled);
            bitStream.Write(MeleeOnly);
        }

        public override void Unpack(EventConnection ps, BitStream bitStream) { }
        public override void Process(EventConnection ps) { }
  
    }
}
