using System;
using System.Collections.Generic;

namespace TNL.Entities
{
    using Data;
    using Notify;
    using Structures;
    using Types;
    using Utils;

    public class GhostRef
    {
        public ulong Mask { get; set; }
        public uint GhostInfoFlags { get; set; }
        public GhostInfo Ghost { get; set; }
        public GhostRef NextRef { get; set; }
        public GhostRef UpdateChain { get; set; }
    }

    public class GhostConnection : EventConnection
    {
        #region Consts

        public const uint GhostIdBitSize = 10U;
        public const uint GhostLookupTableSizeShift = 10U;

        public const uint MaxGhostCount = (1U << (int) GhostIdBitSize);
        public const uint GhostCountBitSize = GhostIdBitSize + 1;

        public const uint GhostLookupTableSize = (1U << (int) GhostLookupTableSizeShift);
        public const uint GhostLookupTableMask = (GhostLookupTableSize - 1);

        #endregion Consts

        protected GhostInfo[] GhostArray;

        protected int GhostZeroUpdateIndex;
        protected int GhostFreeIndex;

        protected bool Ghosting;
        protected bool Scoping;
        protected uint GhostingSequence;
        protected NetObject[] LocalGhosts;
        protected GhostInfo[] GhostRefs;
        protected NetObject ScopeObject;

        public static void RegisterNetClassReps()
        {
            NetEvent.ImplementNetEvent(out RPCStartGhosting.DynClassRep,        "RPC_GhostConnection_rpcStartGhosting",        NetClassMask.NetClassGroupGameMask, 0);
            NetEvent.ImplementNetEvent(out RPCReadyForNormalGhosts.DynClassRep, "RPC_GhostConnection_rpcReadyForNormalGhosts", NetClassMask.NetClassGroupGameMask, 0);
            NetEvent.ImplementNetEvent(out RPCEndGhosting.DynClassRep,          "RPC_GhostConnection_rpcEndGhosting",          NetClassMask.NetClassGroupGameMask, 0);
        }

        public GhostConnection()
        {
            ScopeObject = null;
            GhostingSequence = 0U;
            Ghosting = false;
            Scoping = false;
            GhostArray = null;
            GhostRefs = null;
            LocalGhosts = null;
            GhostZeroUpdateIndex = 0;
        }

        ~GhostConnection()
        {
            ClearAllPacketNotifies();

            if (GhostArray != null)
                ClearGhostInfo();

            DeleteLocalGhosts();
        }

        protected override PacketNotify AllocNotify()
        {
            return new GhostPacketNotify();
        }

        protected override void PacketDropped(PacketNotify note)
        {
            base.PacketDropped(note);

            if (note is not GhostPacketNotify notify)
                throw new ArgumentException("Note must be GhostPacketNotify", nameof(note));

            var packRef = notify.GhostList;
            while (packRef != null)
            {
                var temp = packRef.NextRef;

                var updateFlags = packRef.Mask;

                for (var walk = packRef.UpdateChain; walk != null && updateFlags != 0UL; walk = walk.UpdateChain)
                    updateFlags &= ~walk.Mask;

                if (updateFlags != 0UL)
                {
                    if (packRef.Ghost.UpdateMask == 0UL)
                    {
                        packRef.Ghost.UpdateMask = updateFlags;
                        GhostPushNonZero(packRef.Ghost);
                    }
                    else
                        packRef.Ghost.UpdateMask |= updateFlags;
                }

                if (packRef.Ghost.LastUpdateChain == packRef)
                    packRef.Ghost.LastUpdateChain = null;

                if ((packRef.GhostInfoFlags & (uint) GhostInfoFlags.Ghosting) != 0U)
                {
                    packRef.Ghost.Flags |= (uint) GhostInfoFlags.NotYetGhosted;
                    packRef.Ghost.Flags &= ~(uint) GhostInfoFlags.Ghosting;
                }
                else if ((packRef.GhostInfoFlags & (uint)GhostInfoFlags.KillingGhost) != 0U)
                {
                    packRef.Ghost.Flags |= (uint) GhostInfoFlags.KillGhost;
                    packRef.Ghost.Flags &= ~(uint) GhostInfoFlags.KillingGhost;
                }

                packRef = temp;
            }
        }

        protected override void PacketReceived(PacketNotify note)
        {
            base.PacketReceived(note);

            if (note is not GhostPacketNotify notify)
                throw new ArgumentException("Note must be GhostPacketNotify", nameof(note));

            var packRef = notify.GhostList;

            while (packRef != null)
            {
                if (packRef.Ghost.LastUpdateChain == packRef)
                    packRef.Ghost.LastUpdateChain = null;

                var temp = packRef.NextRef;

                if ((packRef.GhostInfoFlags & (uint) GhostInfoFlags.Ghosting) != 0U)
                {
                    packRef.Ghost.Flags &= ~(uint) GhostInfoFlags.Ghosting;

                    if (packRef.Ghost.Obj != null)
                        packRef.Ghost.Obj.OnGhostAvailable(this);
                }
                else if ((packRef.GhostInfoFlags & (uint) GhostInfoFlags.KillingGhost) != 0U)
                    FreeGhostInfo(packRef.Ghost);

                packRef = temp;
            }
        }

        public override void PrepareWritePacket()
        {
            base.PrepareWritePacket();

            if (!DoesGhostFrom() && !Ghosting)
                return;

            for (var i = 0; i < GhostZeroUpdateIndex; ++i)
            {
                var walk = GhostArray[i];
                ++walk.UpdateSkipCount;

                if ((walk.Flags & (uint) GhostInfoFlags.ScopeLocalAlways) == 0)
                    walk.Flags &= ~(uint) GhostInfoFlags.InScope;
            }

            if (ScopeObject != null)
                ScopeObject.PerformScopeQuery(this);
        }

        protected override void WritePacket(BitStream stream, PacketNotify note)
        {
            base.WritePacket(stream, note);

            if (note is not GhostPacketNotify notify)
                throw new ArgumentException("Note must be GhostPacketNotify", nameof(note));

            if (ConnectionParameters.DebugObjectSizes)
                stream.WriteInt(DebugCheckSum, 32);

            notify.GhostList = null;

            if (!DoesGhostFrom())
                return;

            if (!stream.WriteFlag(Ghosting && ScopeObject != null))
                return;

            for (var i = GhostZeroUpdateIndex - 1; i >= 0; --i)
            {
                if ((GhostArray[i].Flags & (uint) GhostInfoFlags.InScope) == 0)
                    DetachObject(GhostArray[i]);
            }

            var maxIndex = 0U;
            for (var i = GhostZeroUpdateIndex - 1; i >= 0; --i)
            {
                var walk = GhostArray[i];
                if (walk.Index > maxIndex)
                    maxIndex = walk.Index;

                if ((walk.Flags & (uint) GhostInfoFlags.KillGhost) != 0U &&
                    (walk.Flags & (uint) GhostInfoFlags.NotYetGhosted) != 0U)
                {
                    FreeGhostInfo(walk);
                    continue;
                }

                if ((walk.Flags & (uint) (GhostInfoFlags.KillingGhost | GhostInfoFlags.Ghosting)) == 0U)
                    walk.Priority = (walk.Flags & (uint) GhostInfoFlags.KillGhost) != 0U ? 10000.0f : walk.Obj.GetUpdatePriority(ScopeObject, walk.UpdateMask, (int) walk.UpdateSkipCount);
                else
                    walk.Priority = 0.0f;
            }

            GhostRef updateList = null;

            var list = new List<GhostInfo>();
            for (var i = 0; i < GhostZeroUpdateIndex; ++i)
                list.Add(GhostArray[i]);
            
            list.Sort(new GhostInfoComparer());

            for (var i = 0; i < list.Count; ++i)
            {
                GhostArray[i] = list[i];
                GhostArray[i].ArrayIndex = i;
            }

            var sendSize = 1;

            while ((maxIndex >>= 1) != 0)
                ++sendSize;

            if (sendSize < 3)
                sendSize = 3;

            stream.WriteInt((uint) sendSize - 3U, 3);

            for (var i = GhostZeroUpdateIndex - 1; i >= 0 && !stream.IsFull(); --i)
            {
                var walk = GhostArray[i];
                if ((walk.Flags & (uint) (GhostInfoFlags.KillingGhost | GhostInfoFlags.Ghosting)) != 0U)
                    continue;

                var updateStart = stream.GetBitPosition();
                var updateMask = walk.UpdateMask;
                var retMask = 0UL;

                stream.WriteFlag(true);
                stream.WriteInt(walk.Index, (byte) sendSize);

                if (!stream.WriteFlag((walk.Flags & (uint) GhostInfoFlags.KillGhost) != 0U))
                {
                    if (ConnectionParameters.DebugObjectSizes)
                        stream.AdvanceBitPosition(BitStreamPosBitSize);

                    var startPos = stream.GetBitPosition();

                    if ((walk.Flags & (uint) GhostInfoFlags.NotYetGhosted) != 0U)
                    {
                        var classId = walk.Obj.GetClassId(GetNetClassGroup());
                        stream.WriteClassId(classId, (uint) NetClassType.NetClassTypeObject, (uint) GetNetClassGroup());
                        NetObject.PIsInitialUpdate = true;
                    }

                    retMask = walk.Obj.PackUpdate(this, updateMask, stream);

                    if (NetObject.PIsInitialUpdate)
                    {
                        NetObject.PIsInitialUpdate = false;
                        walk.Obj.GetClassRep().AddInitialUpdate(stream.GetBitPosition() - startPos);
                    }
                    else
                        walk.Obj.GetClassRep().AddPartialUpdate(stream.GetBitPosition() - startPos);

                    if (ConnectionParameters.DebugObjectSizes)
                        stream.WriteIntAt(stream.GetBitPosition(), BitStreamPosBitSize, startPos - BitStreamPosBitSize);
                }

                if (stream.GetBitSpaceAvailable() < MinimumPaddingBits)
                {
                    stream.SetBitPosition(updateStart);
                    stream.ClearError();
                    break;
                }

                var upd = new GhostRef
                {
                    NextRef = updateList
                };

                updateList = upd;

                if (walk.LastUpdateChain != null)
                    walk.LastUpdateChain.UpdateChain = upd;

                walk.LastUpdateChain = upd;

                upd.Ghost = walk;
                upd.GhostInfoFlags = 0U;
                upd.UpdateChain = null;

                if ((walk.Flags & (uint) GhostInfoFlags.KillGhost) != 0U)
                {
                    walk.Flags &= ~(uint) GhostInfoFlags.KillGhost;
                    walk.Flags |= (uint) GhostInfoFlags.KillingGhost;
                    walk.UpdateMask = 0UL;
                    upd.Mask = updateMask;
                    GhostPushToZero(walk);
                    upd.GhostInfoFlags = (uint) GhostInfoFlags.KillingGhost;
                }
                else
                {
                    if ((walk.Flags & (uint) GhostInfoFlags.NotYetGhosted) != 0U)
                    {
                        walk.Flags &= ~(uint) GhostInfoFlags.NotYetGhosted;
                        walk.Flags |= (uint) GhostInfoFlags.Ghosting;
                        upd.GhostInfoFlags = (uint) GhostInfoFlags.Ghosting;
                    }

                    walk.UpdateMask = retMask;
                    if (retMask == 0UL)
                        GhostPushToZero(walk);

                    upd.Mask = updateMask & ~retMask;
                    walk.UpdateSkipCount = 0U;
                }
            }

            stream.WriteFlag(false);
            notify.GhostList = updateList;
        }

        protected override void ReadPacket(BitStream stream)
        {
            base.ReadPacket(stream);

            if (ConnectionParameters.DebugObjectSizes)
            {
                var sum = stream.ReadInt(32);
                Console.WriteLine("Assert({0} == {1} || Invalid checksum.", sum, DebugCheckSum);
            }

            if (!DoesGhostTo())
                return;

            if (!stream.ReadFlag())
                return;

            var idSize = (int) stream.ReadInt(3) + 3;

            while (stream.ReadFlag())
            {
                var index = stream.ReadInt((byte) idSize);

                if (stream.ReadFlag())
                {
                    if (LocalGhosts[index] != null)
                    {
                        LocalGhosts[index].OnGhostRemove();
                        LocalGhosts[index] = null;
                    }
                }
                else
                {
                    var endPos = 0U;
                    if (ConnectionParameters.DebugObjectSizes)
                        endPos = stream.ReadInt(BitStreamPosBitSize);

                    if (LocalGhosts[index] == null)
                    {
                        var classId = stream.ReadClassId((uint) NetClassType.NetClassTypeObject, (uint) GetNetClassGroup());
                        if (classId == 0xFFFFFFFFU)
                        {
                            SetLastError("Invalid packet.");
                            return;
                        }

                        if (Create((uint)GetNetClassGroup(), (uint)NetClassType.NetClassTypeObject, (int)classId) is not NetObject obj)
                        {
                            SetLastError("Invalid packet.");
                            return;
                        }

                        obj.SetOwningConnection(this);
                        obj.SetNetFlags(NetFlag.IsGhost);
                        obj.SetNetIndex(index);

                        LocalGhosts[index] = obj;

                        NetObject.PIsInitialUpdate = true;

                        LocalGhosts[index].UnpackUpdate(this, stream);

                        NetObject.PIsInitialUpdate = false;

                        if (!obj.OnGhostAdd(this))
                        {
                            if (ErrorBuffer[0] == 0)
                                SetLastError("Invalid packet.");

                            return;
                        }

                        if (RemoteConnection != null)
                        {
                            if (RemoteConnection is not GhostConnection gc)
                                return;

                            obj.SetServerObject(gc.ResolveGhostParent((int) index));
                        }
                    }
                    else
                        LocalGhosts[index].UnpackUpdate(this, stream);

                    if (ConnectionParameters.DebugObjectSizes)
                        Console.WriteLine("Assert({0} == {1} || unpackUpdate did not match packUpdate for object of class {0}. Expected {1} bits, got {2} bits.", LocalGhosts[index].GetClassName(), endPos, stream.GetBitPosition());

                    if (ErrorBuffer[0] != 0)
                        return;
                }
            }
        }

        public override bool IsDataToTransmit()
        {
            return base.IsDataToTransmit() || GhostZeroUpdateIndex != 0U;
        }

        protected void ClearGhostInfo()
        {
            for (var walk = NotifyQueueHead; walk != null; walk = walk.NextPacket)
            {
                if (walk is not GhostPacketNotify note)
                    throw new Exception("Note must be GhostPacketNotify");

                var delWalk = note.GhostList;
                note.GhostList = null;

                while (delWalk != null)
                {
                    var next = delWalk.NextRef;

                    delWalk.Ghost = null;
                    delWalk.NextRef = null;
                    delWalk.UpdateChain = null;

                    delWalk = next;
                }
            }

            for (var i = 0; i < MaxGhostCount; ++i)
            {
                if (GhostRefs[i].ArrayIndex >= GhostFreeIndex)
                    continue;

                DetachObject(GhostRefs[i]);

                GhostRefs[i].LastUpdateChain = null;

                FreeGhostInfo(GhostRefs[i]);
            }
        }

        protected void DeleteLocalGhosts()
        {
            if (LocalGhosts == null)
                return;

            for (var i = 0; i < MaxGhostCount; ++i)
            {
                if (LocalGhosts[i] != null)
                {
                    LocalGhosts[i].OnGhostRemove();
                    LocalGhosts[i] = null;
                }
            }
        }

        protected bool ValidateGhostArray()
        {
            return true;
        }

        protected void FreeGhostInfo(GhostInfo ghost)
        {
            if (ghost.ArrayIndex < GhostZeroUpdateIndex)
            {
                ghost.UpdateMask = 0UL;
                GhostPushToZero(ghost);
            }

            GhostPushZeroToFree(ghost);
        }

        protected virtual void OnStartGhosting()
        {
        }

        protected virtual void OnEndGhosting()
        {
        }

        public void SetGhostFrom(bool ghostFrom)
        {
            if (GhostArray != null)
                return;

            if (!ghostFrom)
                return;

            GhostFreeIndex = GhostZeroUpdateIndex = 0;
            GhostArray = new GhostInfo[MaxGhostCount];
            GhostRefs = new GhostInfo[MaxGhostCount];

            for (var i = 0U; i < MaxGhostCount; ++i)
            {
                GhostRefs[i] = new GhostInfo
                {
                    Obj = null,
                    Index = i,
                    UpdateMask = 0UL
                };
            }
        }

        public void SetGhostTo(bool ghostTo)
        {
            if (LocalGhosts != null)
                return;

            if (ghostTo)
                LocalGhosts = new NetObject[MaxGhostCount];
        }

        public bool DoesGhostFrom()
        {
            return GhostArray != null;
        }

        public bool DoesGhostTo()
        {
            return LocalGhosts != null;
        }

        public uint GetGhostingSequence()
        {
            return GhostingSequence;
        }

        public void SetScopeObject(NetObject obj)
        {
            if (ScopeObject == obj)
                return;

            obj.SetOwningConnection(this);
            ScopeObject = obj;
        }

        public NetObject GetScopeObject()
        {
            return ScopeObject;
        }

        public void ObjectInScope(NetObject obj)
        {
            if (!Scoping || !DoesGhostFrom())
                return;

            if (!obj.IsGhostable() || (obj.IsScopeLocal() && !IsLocalConnection()))
                return;

            for (var i = 0; i < MaxGhostCount; ++i)
            {
                if (GhostArray[i] != null && GhostArray[i].Obj == obj)
                {
                    GhostArray[i].Flags |= (uint) GhostInfoFlags.InScope;
                    return;
                }
            }

            if (GhostFreeIndex == MaxGhostCount)
                return;

            var gi = GhostArray[GhostFreeIndex];
            GhostPushFreeToZero(gi);
            gi.UpdateMask = 0xFFFFFFFFFFFFFFFFUL;
            GhostPushNonZero(gi);

            gi.Flags = (uint) (GhostInfoFlags.NotYetGhosted | GhostInfoFlags.InScope);
            gi.Obj = obj;
            gi.LastUpdateChain = null;
            gi.UpdateSkipCount = 0U;
            gi.Connection = this;
            gi.NextObjectRef = obj.GetFirstObjectRef();

            if (obj.GetFirstObjectRef() != null)
                obj.GetFirstObjectRef().PrevObjectRef = gi;

            gi.PrevObjectRef = null;
            obj.SetFirstObjectRef(gi);
        }

        public void ObjectLocalScopeAlways(NetObject obj)
        {
            if (!DoesGhostFrom())
                return;

            ObjectInScope(obj);

            for (var i = 0; i < MaxGhostCount; ++i)
            {
                if (GhostArray[i] != null && GhostArray[i].Obj == obj)
                {
                    GhostArray[i].Flags |= (uint) GhostInfoFlags.ScopeLocalAlways;
                    return;
                }
            }
        }

        public void ObjectLocalClearAlways(NetObject obj)
        {
            if (!DoesGhostFrom())
                return;

            for (var i = 0; i < MaxGhostCount; ++i)
            {
                if (GhostArray[i] != null && GhostArray[i].Obj == obj)
                {
                    GhostArray[i].Flags &= ~(uint) GhostInfoFlags.ScopeLocalAlways;
                    return;
                }
            }
        }

        public NetObject ResolveGhost(int id)
        {
            return id == -1 ? null : LocalGhosts[id];
        }

        public NetObject ResolveGhostParent(int id)
        {
            return GhostRefs[id].Obj;
        }

        public void GhostPushNonZero(GhostInfo info)
        {
            if (info.ArrayIndex != GhostZeroUpdateIndex)
            {
                GhostArray[GhostZeroUpdateIndex].ArrayIndex = info.ArrayIndex;
                GhostArray[info.ArrayIndex] = GhostArray[GhostZeroUpdateIndex];
                GhostArray[GhostZeroUpdateIndex] = info;
                info.ArrayIndex = GhostZeroUpdateIndex;
            }
            ++GhostZeroUpdateIndex;
        }

        public void GhostPushToZero(GhostInfo info)
        {
            --GhostZeroUpdateIndex;

            if (info.ArrayIndex == GhostZeroUpdateIndex)
                return;

            GhostArray[GhostZeroUpdateIndex].ArrayIndex = info.ArrayIndex;
            GhostArray[info.ArrayIndex] = GhostArray[GhostZeroUpdateIndex];
            GhostArray[GhostZeroUpdateIndex] = info;
            info.ArrayIndex = GhostZeroUpdateIndex;
        }

        public void GhostPushZeroToFree(GhostInfo info)
        {
            --GhostFreeIndex;

            if (info.ArrayIndex != GhostFreeIndex)
            {
                GhostArray[GhostFreeIndex].ArrayIndex = info.ArrayIndex;
                GhostArray[info.ArrayIndex] = GhostArray[GhostFreeIndex];
                GhostArray[GhostFreeIndex] = info;
                info.ArrayIndex = GhostFreeIndex;
            }
        }

        public void GhostPushFreeToZero(GhostInfo info)
        {
            if (info.ArrayIndex != GhostFreeIndex)
            {
                GhostArray[GhostFreeIndex].ArrayIndex = info.ArrayIndex;
                GhostArray[info.ArrayIndex] = GhostArray[GhostFreeIndex];
                GhostArray[GhostFreeIndex] = info;
                info.ArrayIndex = GhostFreeIndex;
            }

            ++GhostFreeIndex;
        }

        public int GetGhostIndex(NetObject obj)
        {
            if (obj == null)
                return -1;

            if (!DoesGhostFrom())
                return (int) obj.GetNetIndex();

            for (var i = 0; i < MaxGhostCount; ++i)
                if (GhostArray[i] != null && GhostArray[i].Obj == obj && (GhostArray[i].Flags & (uint) (GhostInfoFlags.KillingGhost | GhostInfoFlags.Ghosting | GhostInfoFlags.NotYetGhosted | GhostInfoFlags.KillGhost)) == 0U)
                    return (int) GhostArray[i].Index;

            return -1;
        }

        public bool IsGhostAvailable(NetObject obj)
        {
            return GetGhostIndex(obj) != -1;
        }

        public void ResetGhosting()
        {
            if (!DoesGhostFrom())
                return;

            Ghosting = false;
            Scoping = false;

            rpcEndGhosting();

            ++GhostingSequence;

            ClearGhostInfo();
        }

        public void ActivateGhosting()
        {
            if (!DoesGhostFrom())
                return;

            ++GhostingSequence;

            for (var i = 0; i < MaxGhostCount; ++i)
            {
                GhostArray[i] = GhostRefs[i];
                GhostArray[i].ArrayIndex = i;
            }

            Scoping = true;
            rpcStartGhosting(GhostingSequence);
        }

        public bool IsGhosting()
        {
            return Ghosting;
        }

        public void DetachObject(GhostInfo info)
        {
            info.Flags |= (uint) GhostInfoFlags.KillGhost;

            if (info.UpdateMask == 0UL)
            {
                info.UpdateMask = 0xFFFFFFFFFFFFFFFFUL;
                GhostPushNonZero(info);
            }

            if (info.Obj == null)
                return;

            if (info.PrevObjectRef != null)
                info.PrevObjectRef.NextObjectRef = info.NextObjectRef;
            else
                info.Obj.SetFirstObjectRef(info.NextObjectRef);

            if (info.NextObjectRef != null)
                info.NextObjectRef.PrevObjectRef = info.PrevObjectRef;

            info.PrevObjectRef = info.NextObjectRef = null;
            info.Obj = null;
        }

        private class GhostInfoComparer : IComparer<GhostInfo>
        {
            public int Compare(GhostInfo a, GhostInfo b)
            {
                var ret = a.Priority - b.Priority;
                return ret < 0.0f ? -1 : (ret > 0.0f ? 1 : 0);
            }
        }

#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable IDE0051 // Remove unused private members
        #region RPC Calls

        public void rpcStartGhosting(uint sequence)
        #region rpcStartGhosting
        {
            var rpcEvent = new RPCStartGhosting();
            rpcEvent.Functor.Set(new object[] { sequence });

            PostNetEvent(rpcEvent);
        }


        private void rpcStartGhosting_remote(uint sequence)
        #endregion
        {
            if (!DoesGhostTo())
            {
                SetLastError("Invalid packet.");
                return;
            }

            OnStartGhosting();
            rpcReadyForNormalGhosts(sequence);
        }

        public void rpcReadyForNormalGhosts(uint sequence)
        #region rpcReadyForNormalGhosts
        {
            var rpcEvent = new RPCReadyForNormalGhosts();
            rpcEvent.Functor.Set(new object[] { sequence });

            PostNetEvent(rpcEvent);
        }

        private void rpcReadyForNormalGhosts_remote(uint sequence)
        #endregion
        {
            if (!DoesGhostFrom())
            {
                SetLastError("Invalid packet.");
                return;
            }

            if (sequence == GhostingSequence)
                Ghosting = true;
        }

        public void rpcEndGhosting()
        #region rpcEndGhosting
        {
            var rpcEvent = new RPCEndGhosting();
            rpcEvent.Functor.Set(Array.Empty<object>());

            PostNetEvent(rpcEvent);
        }

        private void rpcEndGhosting_remote()
        #endregion
        {
            if (!DoesGhostTo())
            {
                SetLastError("Invalid packet.");
                return;
            }

            DeleteLocalGhosts();
            OnEndGhosting();
        }
        #endregion

        #region RPC Classes
        private class RPCStartGhosting : RPCEvent
        {
            public static NetClassRepInstance<RPCStartGhosting> DynClassRep;
            public RPCStartGhosting() : base(RPCGuaranteeType.RPCGuaranteedOrdered, RPCDirection.RPCDirAny)
            { Functor = new FunctorDecl<GhostConnection>("rpcStartGhosting_remote", new[] { typeof(uint) }); }
            public override bool CheckClassType(object obj) { return (obj as GhostConnection) != null; }
            public override NetClassRep GetClassRep() { return DynClassRep; }
        }

        private class RPCReadyForNormalGhosts : RPCEvent
        {
            public static NetClassRepInstance<RPCReadyForNormalGhosts> DynClassRep;
            public RPCReadyForNormalGhosts() : base(RPCGuaranteeType.RPCGuaranteedOrdered, RPCDirection.RPCDirAny)
            { Functor = new FunctorDecl<GhostConnection>("rpcReadyForNormalGhosts_remote", new[] { typeof(uint) }); }
            public override bool CheckClassType(object obj) { return (obj as GhostConnection) != null; }
            public override NetClassRep GetClassRep() { return DynClassRep; }
        }

        private class RPCEndGhosting : RPCEvent
        {
            public static NetClassRepInstance<RPCEndGhosting> DynClassRep;
            public RPCEndGhosting() : base(RPCGuaranteeType.RPCGuaranteedOrdered, RPCDirection.RPCDirAny)
            { Functor = new FunctorDecl<GhostConnection>("rpcEndGhosting_remote", Array.Empty<Type>()); }
            public override bool CheckClassType(object obj) { return (obj as GhostConnection) != null; }
            public override NetClassRep GetClassRep() { return DynClassRep; }
        }
        #endregion
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore IDE0051 // Remove unused private members
    }
}
