using System;
using System.Collections.Generic;
using System.Text;
using Qserver.Util;

namespace Qserver.GameServer.Qpang
{
    public class GameConnection : TNL.Entities.EventConnection
    {
        private Player _player;

        public Player Player
        {
            get { return this._player; }
            set { this._player = value; }
        }

        public GameConnection()
        {
            SetPingTimeouts(5000, 10);
            SetFixedRateParameters(50, 50, 1000, 1000);
            SetIsConnectionToClient();
        }

        public void OnConnectionEstablished()
        {
            SetIsConnectionToClient();
        }

        public override void OnConnectionTerminated(TNL.Entities.TerminationReason reason, string msg)
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
            //PostNetEvent(new GCRoomInfo(room));

            //UpdateRoom(room, room.PointsGame ? 4 : 20, room.PointsGame ? room.ScorePoints : room.ScoreTime);
        }

        public void PostNetEvent(GameNetEvent e)
        {
            // TODO
        }

    }
}
