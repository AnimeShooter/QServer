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
        private uint _ip;
        private ushort _port;
        private Player _player;
        private static NetClassRepInstance<GameConnection> _dynClassRep;
        private static NetConnectionRep _connRep;

        public uint Ip
        {
            get { return this._ip; }
            set { this._ip = value; }
        }

        public ushort Port
        {
            get { return this._port; }
            set { this._port = value; }
        }

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
            CGPvEAreaTrigger.RegisterNetClassReps();
            CGPvEEventObject.RegisterNetClassReps();
            CGPvEFloor.RegisterNetClassReps();
            CGPvEGetItem.RegisterNetClassReps();
            CGPvEHitN2P.RegisterNetClassReps();
            CGPvEHitNpcToObject.RegisterNetClassReps();
            CGPvEHitNpc.RegisterNetClassReps();
            CGPvEHitObject.RegisterNetClassReps();
            CGPvERespawnReq.RegisterNetClassReps();
            CGPvERoundStart.RegisterNetClassReps();
            CGPvEStart.RegisterNetClassReps();
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
            GCHit.RegisterNetClassReps();
            GCHitEssence.RegisterNetClassReps();
            GCJoin.RegisterNetClassReps();
            GCMapObject.RegisterNetClassReps();
            GCMasterLog.RegisterNetClassReps();
            GCMesg.RegisterNetClassReps();
            GCMotion.RegisterNetClassReps();
            GCMove.RegisterNetClassReps();
            GCPlayerChange.RegisterNetClassReps();
            GCPvEAreaTriggerInit.RegisterNetClassReps();
            GCPvEDestroyObject.RegisterNetClassReps();
            GCPvEDestroyParts.RegisterNetClassReps();
            GCPvEDieNpc.RegisterNetClassReps();
            GCPvEDoor.RegisterNetClassReps();
            GCPvEEventObject.RegisterNetClassReps();
            GCPvEHitN2N.RegisterNetClassReps();
            GCPvEHitN2P.RegisterNetClassReps();
            GCPvEHitNpc.RegisterNetClassReps();
            GCPvEHitNpcToObject.RegisterNetClassReps();
            GCPvEHitObject.RegisterNetClassReps();
            GCPvEItemInit.RegisterNetClassReps();
            GCPvEMoveNodesNpc.RegisterNetClassReps();
            GCPvEMoveNpc.RegisterNetClassReps();
            GCPvENewRound.RegisterNetClassReps();
            GCPvENpcInit.RegisterNetClassReps();
            GCPvEObjectInit.RegisterNetClassReps();
            GCPvEObjectMove.RegisterNetClassReps();
            GCPvERoundEnd.RegisterNetClassReps();
            GCPvEScoreResult.RegisterNetClassReps();
            GCPvEShootN2P.RegisterNetClassReps();
            GCPvESpecialAttack.RegisterNetClassReps();
            GCPvEStart.RegisterNetClassReps();
            GCPvEUserInit.RegisterNetClassReps();
            GCQuest.RegisterNetClassReps();
            GCReady.RegisterNetClassReps();
            GCRespawn.RegisterNetClassReps();
            GCRoom.RegisterNetClassReps();
            GCRoomInfo.RegisterNetClassReps();
            GCScore.RegisterNetClassReps();
            GCScoreResult.RegisterNetClassReps();
            GCShoot.RegisterNetClassReps();
            GCStart.RegisterNetClassReps();
            GCSync.RegisterNetClassReps();
            GCTarget.RegisterNetClassReps();
            GCWeapon.RegisterNetClassReps();
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

            //SetFixedRateParameters(50, 50, 1000, 1000);
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

            
            //UpdateRoom(room, 26, 1); // enable skills by default
            room.SetSkillsEnabled(true);

            //UpdateRoom(room, 3, 2); // mode, 2
            //UpdateRoom(room, 3, 1); // mode, 2
        }

        public void UpdateRoom(Room room, uint cmd, uint val)
        {
            PostNetEvent(new GCRoom(this._player.PlayerId, cmd, val, room));
        }

        public void StartLoading(Room room, RoomPlayer roomPlayer, bool pve = false)
        {
            if(pve)
            {
                PostNetEvent(new GCPvEStart(room, this._player.PlayerId));
                PostNetEvent(new GCPvEUserInit(roomPlayer));
                PostNetEvent(new GCGameState(this._player.PlayerId, 12));

                //PostNetEvent(new GCJoin(roomPlayer));
                //PostNetEvent(new GCPvENewRound()); // broadcast when round completed


                //PostNetEvent(new GCPvENewRound());

                //PostNetEvent(new GCPvEUserInit(roomPlayer));

                //PostNetEvent(new GCGameState(this._player.PlayerId, 12));

                //PostNetEvent(new GCJoin(roomPlayer));

                // GCPvEUserInit -> GCGameState 12


                //PostNetEvent(new GCPvENewRound());

                //PostNetEvent(new GCPvEUserInit(roomPlayer));

                ////PostNetEvent(new GCGameState(this._player.PlayerId, 12)); // 6 is set for some reason (27 also used)
            }
            else
            {
                PostNetEvent(new GCStart(room, this._player.PlayerId));
                PostNetEvent(new GCJoin(roomPlayer));
                PostNetEvent(new GCGameState(this._player.PlayerId, 12));
            }

            //PostNetEvent(new GCGameState(this._player.PlayerId, 22)); // test hack state
        }

        public void StartSpectating(Room room, RoomPlayer roomPlayer)
        {
            PostNetEvent(new GCRoomInfo(room, true));
            PostNetEvent(new GCStart(room, this._player.PlayerId));
            PostNetEvent(new GCJoin(roomPlayer, true));
            PostNetEvent(new GCGameState(this._player.PlayerId, 12));
        }

        public void StartGameButNotReady()
        {
            PostNetEvent(new GCGameState(this._player.PlayerId, 3));
            PostNetEvent(new GCPlayerChange(this._player, 0, 0));
        }

        public void AddSession(RoomSessionPlayer session)
        {
            PostNetEvent(new GCArrangedAccept(this._player.PlayerId, session.Player.PlayerId, ""));
            PostNetEvent(new GCJoin(session, this._player.RoomPlayer.Spectating));
            PostNetEvent(new GCRespawn(session.Player.PlayerId, session.Character, 1));
        }

        public void SpawnEssence(Spawn spawn)
        {
            PostNetEvent(new GCRespawn(0, 3, 5, spawn.X, spawn.Y, spawn.Z));
        }

        public void DropEssence(Spawn spawn)
        {
            // empty ;P
        }
    }
}
