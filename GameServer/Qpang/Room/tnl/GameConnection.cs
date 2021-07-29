using System;
using System.Collections.Generic;
using System.Text;
using Qserver.Util;
using TNL.Data;
using TNL.Entities;
using TNL.Types;

namespace Qserver.GameServer.Qpang
{
    public class GameConnection : EventConnection
    {
        private Player _player;
        private static NetClassRepInstance<GameConnection> _dynClassRep;
        private static NetConnectionRep _connRep;

        public Player Player
        {
            get { return this._player; }
            set { this._player = value; }
        }

        public static void RegisterNetClassReps()
        {
            ImplementNetConnection(out _dynClassRep, out _connRep, true);

            // Client
            CCCharm.RegisterNetClassReps();
            CCUserInfo.RegisterNetClassReps();
            CGArrangedComplete.RegisterNetClassReps();
            CGArrangedReject.RegisterNetClassReps();
            CGAuth.RegisterNetClassReps();
            CGCard.RegisterNetClassReps();
            CGCharm.RegisterNetClassReps();
            CGEssence.RegisterNetClassReps();
            CGExit.RegisterNetClassReps();
            CGGameItem.RegisterNetClassReps();
            CGGameState.RegisterNetClassReps();
            CGHit.RegisterNetClassReps();
            CGHitEssence.RegisterNetClassReps();
            CGLog.RegisterNetClassReps();
            CGMapObject.RegisterNetClassReps();
            CGMesg.RegisterNetClassReps();
            CGMotion.RegisterNetClassReps();
            CGMove.RegisterNetClassReps();
            CGMoveReport.RegisterNetClassReps();
            CGPlayerChange.RegisterNetClassReps();
            CGReady.RegisterNetClassReps();
            CGRespawn.RegisterNetClassReps();
            CGRoom.RegisterNetClassReps();
            CGRoomInfo.RegisterNetClassReps();
            CGScore.RegisterNetClassReps();
            CGShoot.RegisterNetClassReps();
            CGShootReport.RegisterNetClassReps();
            CGStart.RegisterNetClassReps();
            CGSync.RegisterNetClassReps();
            CGTarget.RegisterNetClassReps();
            CGWeapon.RegisterNetClassReps();
            CSRttRequest.RegisterNetClassReps();

            // Server
            GCArrangedAccept.RegisterNetClassReps();
            GCArrangedConn.RegisterNetClassReps();
            GCCard.RegisterNetClassReps();
            GCCharm.RegisterNetClassReps();
            GCDisconnect.RegisterNetClassReps();
            GCEssence.RegisterNetClassReps();
            GCExit.RegisterNetClassReps();
            GCGameItem.RegisterNetClassReps();
            GCGameState.RegisterNetClassReps();
            GCHitEssence.RegisterNetClassReps();

            GCJoin.RegisterNetClassReps();
            GCRoom.RegisterNetClassReps();
            GCRoomInfo.RegisterNetClassReps();

            GGReload.RegisterNetClassReps();
            P_CSRttReport.RegisterNetClassReps();
            P_CSRttResponse.RegisterNetClassReps();

        }

        public GameConnection()
        {
            //#------------------------------------------------------------------------------
            //
            //## UDP네트워크 타임아웃검사 
            //
            //#------------------------------------------------------------------------------
            //    pingTimeout = 5000	# 밀리초 단위
            //    pingRetryCnt = 10 	# 최대 검사 횟수 
            //#------------------------------------------------------------------------------
            //## flowControl을 auto off 하면 사용하는 값 
            //#------------------------------------------------------------------------------
            //    flowControl = 1 # auto = 1, manual = 0, auto일 경우 아래 설정을 사용하지 않는다. 
            //    minSendPeriod = 50		# Minimum millisecond delay (maximum rate) between packet sends
            //    minRecvPeriod = 50 		# Minimum millisecond delay the remote host should allow between sends
            //    maxSendBandwidth = 1000		# Number of bytes per second we can send over the connection.
            //    maxRecvBandwidth = 1000		# Number of bytes per second max that the remote instance should send.
            //##------------------------------------------------------------------------------

            SetFixedRateParameters(50, 50, 1000, 1000);
            SetPingTimeouts(5000, 10);
            SetIsConnectionToClient();
        }

        public override NetClassRep GetClassRep()
        {
            return _dynClassRep;
        }

        public override NetClassGroup GetNetClassGroup()
        {
            return NetClassGroup.NetClassGroupGame;
        }

        public override void OnConnectionEstablished()
        {
            SetIsConnectionToClient();
        }

        public override void OnConnectionTerminated(TerminationReason reason, string msg)
        {
            if (this._player == null)
                return;

            try
            {
                Game.Instance.RoomServer.DropConnection(this._player.PlayerId);
            }catch(Exception e)
            {
                Log.Message(LogType.ERROR, "GameConnection OnConnectionTerminated " + e.ToString());
            }
        }

        public override void OnConnectTerminated(TerminationReason reason, string msg)
        {
            if (this._player == null)
                return;

            try
            {
                Game.Instance.RoomServer.DropConnection(this._player.PlayerId);
            }
            catch (Exception e)
            {
                Log.Message(LogType.ERROR, "GameConnection OnConnectTerminated " + e.ToString());
            }
        }

        public void EnterRoom(Room room)
        {
            PostNetEvent(new GCRoom(this._player.PlayerId, 9, room));
            PostNetEvent(new GCRoomInfo(room));

            UpdateRoom(room, room.PointsGame ? (uint)4 : (uint)20, room.PointsGame ? room.ScorePoints : room.ScoreTime);
            UpdateRoom(room, 26, 0); // t s
        }

        public void UpdateRoom(Room room, uint cmd, uint val)
        {
            PostNetEvent(new GCRoom(this._player.PlayerId, cmd, val, room));
        }

        public void StartLoading(Room room, RoomPlayer player)
        {
            // TODO
        }

        public void StartSpectating(Room room, RoomPlayer roomPlayer)
        {
            // TODO
        }

        public void StartGameButNotReady()
        {
            // TODO
        }

        public void AddSession(RoomSessionPlayer session)
        {
            // TODO
        }

        public void SpawnEssence(Spawn spawn)
        {
            // TODO
        }

        public void DropEssence(Spawn spawn)
        {

        }
    }
}
