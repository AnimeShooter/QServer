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
            CGRoom.RegisterNetClassReps();
            CGRoomInfo.RegisterNetClassReps();

            CSRttRequest.RegisterNetClassReps();

            // shit switches here bro
            GCArrangedAccept.RegisterNetClassReps();
            GCArrangedConn.RegisterNetClassReps();
            GCCard.RegisterNetClassReps();
            GCCharm.RegisterNetClassReps();
            GCDisconnect.RegisterNetClassReps();
            GCEssence.RegisterNetClassReps();
            GCExit.RegisterNetClassReps();
            GCGameItem.RegisterNetClassReps();
            GCGameState.RegisterNetClassReps();

            GCJoin.RegisterNetClassReps();
            GCRoom.RegisterNetClassReps();
            GCRoomInfo.RegisterNetClassReps();

            GGReload.RegisterNetClassReps();
            P_CSRttReport.RegisterNetClassReps();
            P_CSRttResponse.RegisterNetClassReps();

        }

        public GameConnection()
        {
            SetPingTimeouts(5000, 10);
            SetFixedRateParameters(50, 50, 1000, 1000);
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

        public override void OnConnectTerminated(TNL.Entities.TerminationReason reason, string msg)
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
