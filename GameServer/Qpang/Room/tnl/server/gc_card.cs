using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNL.Entities;
using TNL.Utils;

namespace Qserver.GameServer.Qpang
{
    public class GCCard : GameNetEvent
    {
        public byte cmd;
        public uint uid;
        public uint targetUid;
        public uint itemId;
        public ulong seqId;
        public uint cardType;
        public uint actionType;
        public uint chargePoints;
        public uint skillCount;
        public uint leftCount;
        public uint dataSrcUid;
        public uint dataTargetUid;
        public uint unk01;
        public byte count;

        public GCCard() : base(GameNetId.GC_CARD, GuaranteeType.GuaranteedOrdered, EventDirection.DirAny) { }
        public GCCard(uint playerId, uint targetId, byte cmd, uint cardType, uint itemId, ulong seqId) : base(GameNetId.GC_ARRANGED_CONN, GuaranteeType.GuaranteedOrdered, EventDirection.DirAny)
        {
            this.uid = playerId;
            this.targetUid = targetId;
            this.cmd = cmd;
            this.cardType = cardType;
            this.itemId = itemId;
            this.seqId = seqId;
        }
        public GCCard(uint playerId, uint guagePercentage, uint guagePoints) : base(GameNetId.GC_ARRANGED_CONN, GuaranteeType.GuaranteedOrdered, EventDirection.DirAny)
        {
            this.uid = playerId;
            this.cmd = 8;
            this.cardType = 9;
            this.chargePoints = guagePercentage;
            this.skillCount = guagePoints;
        }

        public override void Pack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Write(cmd);
            bitStream.Write(uid);
            bitStream.Write(targetUid);
            bitStream.Write(itemId);
            bitStream.Write(seqId);
            bitStream.Write(cardType);
            bitStream.Write(actionType);
            bitStream.Write(chargePoints);
            bitStream.Write(skillCount);
            bitStream.Write(leftCount);
            bitStream.Write(dataSrcUid);
            bitStream.Write(dataTargetUid);
            bitStream.Write(unk01);
            bitStream.Write(count);

        }
        public override void Unpack(EventConnection ps, BitStream bitStream) { }
        public override void Process(EventConnection ps) { }
    }
}
