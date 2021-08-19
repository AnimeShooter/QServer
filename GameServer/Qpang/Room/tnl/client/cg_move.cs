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
    public class CGMove : GameNetEvent
    {
        private static NetClassRepInstance<CGMove> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CGMove", NetClassMask.NetClassGroupGameMask, 0);
        }

        public uint PlayerId;
        public uint Cmd;
        /*
         * 0x00000000 pitch/yawn/jump/roll/stop
         * 0x00000100 forward
         * 0x00001000 backwards
         * 0x00010000 left
         * 0x00100000 right
         * 
         */


        public float PosX;
        public float PosY;
        public float PosZ;
        public float unk04;
        public float unk05;
        public float unk06;
        public float Pitch;
        public float Yawn;
        public uint Tick;
        public uint Unk10;
        //public uint unk11;

        public CGMove() : base(GameNetId.CG_MOVE, GuaranteeType.Unguaranteed, EventDirection.DirClientToServer) { }

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream)
        {
            bitStream.Read(out PlayerId);
            bitStream.Read(out Cmd);
            bitStream.Read(out PosX);
            bitStream.Read(out PosY);
            bitStream.Read(out PosZ);
            bitStream.Read(out unk04);
            bitStream.Read(out unk05);
            bitStream.Read(out unk06);
            bitStream.Read(out Pitch);
            bitStream.Read(out Yawn);
            bitStream.Read(out Tick);
            bitStream.Read(out Unk10);
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

            if (roomPlayer.Spectating)
                return;

            var session = roomPlayer.RoomSessionPlayer;
            if (session == null)
                return;

            if (session.Death)
                return;

            session.Position = new Position() { X = PosX, Y = PosY, Z = PosZ };

            var roomSession = roomPlayer.Room.RoomSession;
            
            //Console.WriteLine($"[{DateTime.UtcNow.ToString()}][{player.Name}] {Cmd.ToString("X8")} {PosX} {PosY} {PosY} | {unk04} {unk05} {unk06} {Pitch} {Yawn} --- {Tick} {Unk10.ToString("X8")}");
            
            // update serverside player position
            roomPlayer.RoomSessionPlayer.X = PosX;
            roomPlayer.RoomSessionPlayer.Y = PosY;
            roomPlayer.RoomSessionPlayer.Z = PosZ;

            
            // TODO: fix timing issue and detect cheatingg
            if (roomSession != null)
            {
                //if (roomPlayer.RoomSessionPlayer.FirstSeen == 0)
                //    roomPlayer.RoomSessionPlayer.FirstSeen = (Util.Util.Timestamp() - (roomSession.StartTime)) * 1000;
                //else
                //{
                //    uint serverTick = ;
                //}
                
                //Console.WriteLine($"{Tick} == {serverTick}"); // 45s
                //if (serverTick + 2000 < Tick)
                //    Console.WriteLine($"Speedhack detected on player {roomPlayer.Player.Name}!");

                roomSession.RelayPlayingExcept<GCMove>(player.PlayerId, PlayerId, Cmd, PosX, PosY, PosZ, unk04, unk05, unk06, Pitch, Yawn, Tick, Unk10);
            }
            
                
        }
    }
}
