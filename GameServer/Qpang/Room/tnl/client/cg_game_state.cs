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
    public class CGGameState : GameNetEvent
    {
        private static NetClassRepInstance<CGGameState> _dynClassRep;

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetEvent(out _dynClassRep, "CGGameState", NetClassMask.NetClassGroupGameMask, 0);
        }

		public enum State : uint
		{
			DATA = 0,
			GAME_WAITING_PLAYERS_SPECTATE_UNK = 1,
			GAME_FORCE_LEAVE_STO = 2,
			GAME_WAITING_PLAYERS = 3,
			SEND_GAME_STATE_PLAY = 4,
			DISCONNECT_P2P = 7,
			REMOVE_INVINCIBILITY = 8,
			SYNC_TIME = 11,
			GAME_START = 12,
			GAME_START_PVE = 13, //GAME_START,
			GAME_STATE_P2P_FAIL = 14,
			LEAVE_GAME = 15,
			UPDATE_HEALTH = 16,
			KILLFEED_ADD = 17,
			PLAYER_STATE_HACK = 22,
			GAME_OVER = 23,
			INV_IN = 24,
			INV_OUT = 25,
			// 27 idk
			KILLFEED_ADD_HEAD = 28,
			START_RESPAWN_TIMER = 29,

			PREY_COUNT_START = 31,
			PREY_SELECT = 33,
			PREY_SELECT_RSP = 34,

			PREY_TRANFORM = 36, // aka broadcast
			PREY_TRANFORM_READY = 37,
			/*
             * 3  - Game Loading
             * 9  - ServerGame::broadcastGCGameState
             * 11 - ServerGame::sendGameStatePlay
             * 15 - ServerGame::gotoWaitRoom
             * 22 - ServerGame::playerStateHack
             * 24 - inventory in?
             * 25 - inventory out?
             * 27 - 
             * 29 - START_RESPAWN_TIMER
             * 35 - ServerGame::recv_PublicEnmeyPossible
             * 32 - ?
             * 36 - PREY_TRANFORM
             * 37 - PREY_TRANFORM_READY
             * default: error?
             */

		};

		public uint PlayerId;
		public uint Cmd;
		public uint Data;
		public uint unk04;
		public uint unk05;
		public uint unk06;

		public CGGameState() : base(GameNetId.CG_GAME_STATE, GuaranteeType.Guaranteed, EventDirection.DirClientToServer) { }

        public override void Pack(EventConnection ps, BitStream bitStream) { }
        public override void Unpack(EventConnection ps, BitStream bitStream)
		{
			bitStream.Read(out PlayerId);
			bitStream.Read(out Cmd); // 15
			bitStream.Read(out Data);
			bitStream.Read(out unk04);
			bitStream.Read(out unk05);
			bitStream.Read(out unk06);
		}
        public override void Process(EventConnection ps)
		{
			Post(ps); // NOTE: not processd in PvE?
		}

        public override void Handle(GameConnection conn, Player player)
        {
			var roomPlayer = player.RoomPlayer;
			if (roomPlayer == null)
				return;

			switch((State)Cmd)
            {
				case State.INV_IN:
				case State.INV_OUT:
					roomPlayer.Room.BroadcastWaiting<GCGameState>(player.PlayerId, Cmd, (uint)0, (uint)0);
					break;
				case State.LEAVE_GAME:
					var roomSessionLg = roomPlayer.Room.RoomSession;
					if (roomSessionLg != null)
                    {
						if (!roomSessionLg.RemovePlayer(player.PlayerId))
							conn.PostNetEvent(new GCGameState(player.PlayerId, 15));
					}
					else
						conn.PostNetEvent(new GCGameState(player.PlayerId, 15));

					roomPlayer.Playing = false;
					roomPlayer.Spectating = false;
					roomPlayer.SetReady(false);
					conn.EnterRoom(roomPlayer.Room);
					roomPlayer.Room.SyncPlayers(roomPlayer);
					break;
				case State.GAME_WAITING_PLAYERS:
					var roomSessionGwp = roomPlayer.Room.RoomSession;
					if (roomSessionGwp != null)
						roomSessionGwp.AddPlayer(conn, roomPlayer.Team);
					break;
				// TODO: 35 (after 33?)
				case (State)35:
				case State.PREY_TRANFORM_READY:
					// TODO
				default:
					break;
            }
        }
    }
}
