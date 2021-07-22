using System;
using System.Collections.Generic;
using System.Text;
using TNL.Entities;
using TNL.Utils;

namespace Qserver.GameServer.Qpang
{
    public class GCRoom : GameNetEvent
    {
        public GCRoom() : base(GameNetId.GC_ROOM, GuaranteeType.Guaranteed, EventDirection.DirAny)
        {
            
        }

        public GCRoom(uint playerId, uint cmd, Room room) : base(GameNetId.GC_ROOM, GuaranteeType.Guaranteed, EventDirection.DirAny)
        {
            this.zero = 0;
            this.playerId = playerId;
            this.cmd = cmd;
            this.roomId = room.Id;
            this.mode = (byte)room.Mode;
            this.memberCount = room.PlayerCount;
            this.title = room.Name;
            this.meleeOnly = room.MeleeOnly ? (byte)1 : (byte)0;
            this.skillsEnabled = room.SkillsEnabled ? (byte)1 : (byte)0;
        }
        public GCRoom(uint playerId, uint cmd, uint val, Room room) : base(GameNetId.GC_ROOM, GuaranteeType.Guaranteed, EventDirection.DirAny)
        {
            this.zero = 0;
            this.playerId = playerId;
            this.cmd = cmd;
            this.roomId = room.Id;
            this.mode = (byte)room.Mode;
            this.memberCount = room.PlayerCount;
            this.title = "-";
            this.value = val;
            this.meleeOnly = room.MeleeOnly ? (byte)1 : (byte)0;
            this.skillsEnabled = room.SkillsEnabled ? (byte)1 : (byte)0;
        }

        public override void Pack(EventConnection conn, BitStream bitStream)
        {
            bitStream.Write(playerId);
            bitStream.Write(cmd);
            bitStream.Write(value);

            bitStream.Write(mode);
            bitStream.Write(memberCount);
            bitStream.Write(goal);

            bitStream.WriteString(password);
            bitStream.WriteString(title);
            bitStream.Write(time);
            bitStream.Write(rounds);
            bitStream.Write(_160);

            bitStream.Write(zero);

            bitStream.Write(_161);
            bitStream.Write(skillsEnabled);
            bitStream.Write(meleeOnly);
        }

        public override void Unpack(EventConnection ps, BitStream bitStream) { }
        public override void Process(EventConnection ps) { }

        private uint playerId; // 92
        private uint cmd; // 96

        // union
        private uint value;
        private uint roomId;

        private byte mode;
        private byte memberCount;
        private byte goal = 10;

        string password;
        string title;
        byte time = 6;

        // union
        private byte rounds = 1;
        private byte gameId;

        byte _160 = 1; // in CGRoom

        private uint zero = 0;
        private byte _161 = 0;
        private byte skillsEnabled = 0;
        private byte meleeOnly = 0;  // 166

        
    }
}
