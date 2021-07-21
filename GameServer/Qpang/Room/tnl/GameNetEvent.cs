using System;
using System.Collections.Generic;
using System.Text;
using Qserver.GameServer.Network;
using TNL.Entities;
using Qserver.Util;
using TNL.Utils;

namespace Qserver.GameServer.Qpang
{
    public abstract class GameNetEvent : TNL.Entities.NetEvent
    {
        public GameNetId Id;
        public GameConnection GameConnection;

        public GameNetEvent(GameNetId gameNetId, GuaranteeType guaranteeType, EventDirection eventDirection) : base(guaranteeType, eventDirection)
        {
            this.Id = gameNetId;
        }

        //public string ByteBufferToString(byte[] buffer)
        //{

        //}

        //public override void Pack(EventConnection ps, BitStream stream)
        //{

        //}
        //public override void Unpack(EventConnection ps, BitStream steam)
        //{

        //}
        //public override void Process(EventConnection ps)
        //{

        //}

        public void NotifyPosted(EventConnection e)
        {

        }
        public void NotifyDelivered(EventConnection e)
        {

        }

        public void Post<T>(EventConnection ps)
        {
            this.GameConnection = (GameConnection)ps;
            //Game.Instance.RoomServer.HandleEvent();
        }

        public virtual void Handle(GameConnection conn, Player player)
        {
            Log.Message(LogType.ERROR, $"GameNetEvent Unhandeled {Id}!");
        }
    }
}
