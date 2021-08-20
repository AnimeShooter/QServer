using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNL.Entities;
using TNL.Utils;
using TNL.Data;
using TNL.Types;

namespace Qserver.GameServer.Qpang
{
    public class CGHit : GameNetEvent
    {
        private static NetClassRepInstance<CGHit> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CGHit", NetClassMask.NetClassGroupGameMask, 0);
        }

        public enum HitLocations : byte
        {
            HEAD = 0,
            BODY = 1,
            R_ARM = 2,
            R_HAND = 3,
            L_ARM = 4,
            L_HAND = 5,
            R_LEG = 6,
            R_FEET = 7,
            L_LEG = 8,
            L_FEET = 9
        };

        enum MapObjects : byte
        {
            TRAP_FLAME = 1,
            TRAP_PRESS = 2,
            TRAP_FALL = 3,
            TRAP_DOWN = 4,
            TRAP_POISON_GROUND = 6,
            TRAP_SHIP_PROPELLER = 7
        };

        public uint SrcPlayerId;
        public uint DstPlayerId;
        public uint unk03;
        public float SrcPosX;
        public float SrcPosY;
        public float SrcPosZ;
        public float DstPosX;
        public float DstPosY;
        public float DstPosZ;
        public uint EntityId;
        public byte HitType;
        public byte HitLocation;

        public uint WeaponId;
        public ulong RTT;
        public byte WeaponType;
        public byte unk16;
        public byte unk17;
        public uint unk18;

        public CGHit() : base(GameNetId.CG_HIT, GuaranteeType.Guaranteed, EventDirection.DirClientToServer) { }

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out SrcPlayerId);
            bitStream.Read(out DstPlayerId);
            bitStream.Read(out unk03);
            bitStream.Read(out SrcPosX);
            bitStream.Read(out SrcPosY);
            bitStream.Read(out SrcPosZ);
            bitStream.Read(out DstPosX);
            bitStream.Read(out DstPosY);
            bitStream.Read(out DstPosZ);
            bitStream.Read(out EntityId);
            bitStream.Read(out HitType);
            bitStream.Read(out HitLocation);
            bitStream.Read(out WeaponId);
            bitStream.Read(out RTT);
            bitStream.Read(out WeaponType);
            bitStream.Read(out unk16);
            bitStream.Read(out unk17);
            bitStream.Read(out unk18);
        }

        public override void Handle(GameConnection conn, Player player)
        {
            var roomPlayer = player.RoomPlayer;
            if (roomPlayer == null)
                return;

            var session = roomPlayer.RoomSessionPlayer;
            if (session == null)
                return;

            // TODO
            //if (!session.WeaponManager.HasWeapon(WeaponId) && !IsTrap(WeaponId))
            //    return;

            if (DstPlayerId == 0)
                HitEmpty(WeaponId, session);
            else if (SrcPlayerId == DstPlayerId)
                Hit(WeaponId, session, session, HitType);
            else
            {
                var targetSession = session.RoomSession.Find(DstPlayerId);
                if (targetSession == null)
                    return;

                if (targetSession.Invincible || targetSession.Death)
                    return;

                Hit(WeaponId, session, targetSession, HitType);
            }
        }

        public void Hit(uint weaponId, RoomSessionPlayer srcPlayer, RoomSessionPlayer dstPlayer, byte bodyPart)
        {
            if (dstPlayer.Death)
                return;

            ushort dmg = 0;
            byte effectId = 0;

            var roomSession = srcPlayer.RoomSession;
            var teamMode = roomSession.GameMode.IsTeamMode();
            var sameTeam = teamMode && srcPlayer.Team == dstPlayer.Team;
            var weapon = Game.Instance.WeaponManager.Get(WeaponId);

            if (weapon.WeaponType != Qpang.WeaponType.BOMB && srcPlayer.Death)
                return;

            if(IsTrap(WeaponId))
            {
                dmg = GetTrapDamage(WeaponId);
                if (WeaponId == (uint)MapObjects.TRAP_FALL) // reset on out of map fall
                    if (dstPlayer == roomSession.EssenceHolder)
                        roomSession.ResetEssence();

                dstPlayer.TakeHealth(dmg);
            }
            else
            {
                if(WeaponId != 1095434246) // octo TODO filter all mines
                    if (!srcPlayer.EntityManager.IsValidShot(EntityId))
                        return;

                dmg = weapon.Damage;

                switch((HitLocations)HitLocation)
                {
                    case HitLocations.HEAD:
                        break; // 1
                    case HitLocations.BODY:
                        dmg = (ushort)(dmg * 0.9f);
                        break;
                    case HitLocations.L_LEG:
                    case HitLocations.R_LEG:
                    case HitLocations.L_ARM:
                    case HitLocations.R_ARM:
                        dmg = (ushort)(dmg * 0.8f);
                        break;
                    case HitLocations.L_FEET:
                    case HitLocations.R_FEET:
                    case HitLocations.L_HAND:
                    case HitLocations.R_HAND:
                        dmg = (ushort)(dmg * 0.6f);
                        break;
                }

                if (sameTeam)
                    dmg = 0;

                if (WeaponId == 1095434246) // TODO octo similar stuff
                    if (srcPlayer == dstPlayer)
                        dmg = 0;

                dstPlayer.TakeHealth(dmg);

                Random rnd = new Random();
                var applyEffect = rnd.Next(0, 100) <= weapon.EffectChance;

                if(applyEffect)
                {
                    effectId = weapon.EffectId;
                    dstPlayer.EffectManager.AddEffect(srcPlayer, weapon, EntityId);
                }

                // NOTE: Skill points added
                if (srcPlayer.RoomSession.Room.SkillsEnabled)
                    srcPlayer.SkillManager.AddSkillPoints((uint)(dmg/4));
            }

            var srcId = srcPlayer.Player.PlayerId;
            var dstId = dstPlayer.Player.PlayerId;

            // NOTE: may add anti-cheat manager to verify Start & End location (last position tracking) followed with a LoS check to verify kill
            roomSession.RelayPlaying<GCHit>((uint)SrcPlayerId, (uint)dstPlayer.Player.PlayerId, (uint)unk03, SrcPosX, SrcPosY, SrcPosZ, DstPosX, DstPosY, DstPosZ, (uint)EntityId,
                (byte)HitType, (byte)HitLocation, (ushort)dstPlayer.Health, (ushort)dmg, (uint)WeaponId, (ulong)RTT, (uint)WeaponType, (byte)unk16, (byte)(srcPlayer.Streak + 1), 
                (byte)unk18, (byte)effectId, (byte)0, (byte)0, (byte)0); // NOTE: Health as in abse health?

            if(dstPlayer.Death)
            {
                srcPlayer.EntityManager.AddKill(EntityId);
                roomSession.GameMode.OnPlayerKill(srcPlayer, dstPlayer, weapon, HitLocation);
                roomSession.KillPlayer(srcPlayer, dstPlayer, weapon.ItemId, HitLocation == 0);
                dstPlayer.StartPrespawn(); // prespawn?
            }
        }

        public void HitEmpty(uint weaponId, RoomSessionPlayer srcPlayer)
        {
            var roomSession = srcPlayer.RoomSession;
            roomSession.RelayPlaying<GCHit>(SrcPlayerId, (uint)0, unk03, SrcPosX, SrcPosY, SrcPosZ, DstPosX, DstPosY, DstPosZ, EntityId,
                HitType, HitLocation, (ushort)0, (ushort)0, WeaponId, RTT, (uint)WeaponType, unk16, (byte)(srcPlayer.Streak + 1), (byte)unk18, (byte)0, (byte)0, (byte)0, (byte)0);
        }

        public bool IsTrap(uint weaponId)
        {
            return weaponId == (uint)MapObjects.TRAP_FLAME || weaponId == (uint)MapObjects.TRAP_POISON_GROUND 
                || weaponId == (uint)MapObjects.TRAP_SHIP_PROPELLER || weaponId == (uint)MapObjects.TRAP_DOWN 
                || weaponId == (uint)MapObjects.TRAP_FALL || weaponId == (uint)MapObjects.TRAP_PRESS;
        }

        public ushort GetTrapDamage(uint weaponId)
        {
            switch((MapObjects)weaponId)
            {
                case MapObjects.TRAP_FLAME:
                case MapObjects.TRAP_POISON_GROUND:
                case MapObjects.TRAP_SHIP_PROPELLER:
                    return 10;
                case MapObjects.TRAP_PRESS:
                    return 150;
                case MapObjects.TRAP_DOWN:
                    return 0;
                default:
                case MapObjects.TRAP_FALL:
                    return 999;
            }
        }

        public override void Process(EventConnection ps)
        {
            Post(ps);
        }
    }
}