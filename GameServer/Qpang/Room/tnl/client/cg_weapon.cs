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
    public class CGWeapon : GameNetEvent
    {
        private static NetClassRepInstance<CGWeapon> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CGWeapon", NetClassMask.NetClassGroupGameMask, 0);
        }

        public enum Commands : uint
        {
            SWAP = 0,
            RELOAD = 3,
            ENABLE_SHOOTING = 5
        }

        public uint PlayerId;
        public uint Cmd;
        public uint ItemId;
        public ulong CardId;

        public CGWeapon() : base(GameNetId.CG_WEAPON, GuaranteeType.GuaranteedOrdered, EventDirection.DirClientToServer) { }

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream) 
        {
            bitStream.Read(out PlayerId);
            bitStream.Read(out Cmd);
            bitStream.Read(out ItemId);
            bitStream.Read(out CardId);
        }
        public override void Process(EventConnection ps) 
        {
            Post(ps);
        }

        public override void Handle(GameConnection conn, Player player)
        {
            var roomPlayer = player.RoomPlayer;
            if (roomPlayer == null)
                return;

            var session = roomPlayer.RoomSessionPlayer;
            if (session == null)
                return;

            if(Cmd == (uint)Commands.SWAP)
            {
                if (!session.WeaponManager.HasWeapon(ItemId))
                    return;

                session.WeaponManager.SwitchWeapon(ItemId);
            }
            else if(Cmd == (uint)Commands.RELOAD)
            {
                if (!session.WeaponManager.HasWeapon(ItemId))
                    return;

                if (!session.WeaponManager.CanReload)
                    return;

                session.OnReload();

                session.WeaponManager.Reload(CardId);
            }
        }
    }
}
