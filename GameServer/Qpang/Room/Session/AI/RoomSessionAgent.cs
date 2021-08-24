using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qserver.GameServer.Qpang
{
    public class RoomSessionAgent : RoomSessionPlayer
    {
        private Position _targetLocation;
        private Position _startLocation;

        public RoomSessionAgent(GameConnection conn, RoomSession roomSession, byte team) : base(conn, roomSession, team)
        {

        }

        public override bool IsRobot()
        {
            return true;
        }

        public override void Tick()
        {
            // handle movement
            //if(!base.IsInRange(this._targetLocation, 1f, false))
            //{
            //    base.RoomSession.RelayPlayingExcept<GCMove>(base.Player.PlayerId, )
            //}

            // TODO: float unk04, float unk05, float unk06, float pitch, float yawn, uint tick, uint unk10
            //base.Position = new Position()
            //{
            //    X = base.Position.X + 0.01f,
            //    Y = base.Position.Y,
            //    Z = base.Position.Z
            //};

            // basic idle?
            base.RoomSession.RelayPlayingExcept<GCMove>(base.Player.PlayerId, base.Player.PlayerId, (uint)0x00000000, base.Position.X, base.Position.Y, base.Position.Z, 0f, 0f, 0f,
                -7f, 1.4f, (uint)0, (uint)0);

            // do stuff while alive
            if (!base.Respawning && base.Playing)
            {
                var targets = base.RoomSession.GetPlayingPlayers();
                foreach (var p in targets)
                {
                    if (p.Team == base.Team)
                        continue;

                    if (!p.Playing || p.Respawning)
                        continue;


                    Random rnd = new Random();
                    var shoot = new CGShoot();
                    shoot.ItemId = 1095434240;
                    shoot.SrcX = base.Position.X;
                    shoot.SrcY = base.Position.Y;
                    shoot.SrcZ = base.Position.Z; // TODO: offset based on gun and character!
                    shoot.DstX = p.Position.X;
                    shoot.DstY = p.Position.Y; // rnd.Next(0, 100) / 100f; //p.Position.Y;
                    shoot.DstZ = p.Position.Z; // rnd.Next(0, 100) / 100f;  //p.Position.Z;
                    shoot.EntityId = p.Player.PlayerId; // idk?
                    shoot.Handle(null, base.Player);
                    //var hit = new CGHit();
                    //hit.WeaponId = (uint)1095434246;
                    //hit.HitLocation = 1; // not headshot
                    //hit.Hit(this, p);


                    // Shoot(p.Position); // shoot all players?
                }

            }
            
            base.Tick();
        }

        public void Shoot(Position target)
        {
            // uint playerId, uint itemId, float srcX, float srcY, float srcZ, float dirX, float dirY, float dirZ, uint entityId, uint isP2P

            uint itemId = 0; // TODO: get a real weapon

            // TODO: rebase shooting
            base.RoomSession.RelayPlayingExcept<GCShoot>(base.Player.PlayerId, base.Player.PlayerId, itemId, base.Position.X, base.Position.Y, base.Position.Z, target.X, target.Y, target.Z, (uint)0, (uint)0);


        }

        public void Run(Position target)
        {
            this._startLocation = base.Position;
            this._targetLocation = target;
        }
    }
}
