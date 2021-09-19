using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class GunGame : GameMode
    {
        // NOTE: How it works:
        /*
         * Generate X amount of custom weapons (last is melee)
         * - everytime you kill, you climb the gun ladder
         * - if you get killed with melee, you get downgraded
         * - first one to play out wins!
         */

        private Weapon[] _weapons;
        private Weapon _meleeWeapon;

        public override bool IsMissionMode()
        {
            return true;
        }

        public override void OnStart(RoomSession roomSession)
        {
            // randomize weapons
            this._weapons = new Weapon[roomSession.Room.ScorePoints];
            for(int i = 0; i < this._weapons.Length-1; i++)
                this._weapons[i] = Game.Instance.WeaponManager.GetRandomWeapon();

            // random melee weapon
            this._meleeWeapon = Game.Instance.WeaponManager.GetRandomWeapon(WeaponType.MELEE);
            this._weapons[this._weapons.Length - 1] = this._meleeWeapon;

            base.OnStart(roomSession);
        }

        public override void OnPlayerKill(RoomSessionPlayer killer, RoomSessionPlayer target, Weapon weapon, byte hitLocation)
        {
            if(killer != target)
            {
                if (weapon.WeaponType == WeaponType.MELEE)
                    target.Score--; // degrade target
                killer.AddScore();
                if(killer.Score < killer.RoomSession.Room.ScorePoints)
                {
                    bool matchPoint = killer.Score == killer.RoomSession.Room.ScorePoints - 1; // last point, melee only

                    killer.WeaponManager.Replace(this._weapons[killer.Score]);
                    killer.Post(new GCJoin(killer));
                }
                
            }
            base.OnPlayerKill(killer, target, weapon, hitLocation);
        }

        public override bool IsTeamMode()
        {
            return false;
        }
    }
}
