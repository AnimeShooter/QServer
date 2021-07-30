using System;
using System.Collections.Generic;
using System.Text;
using Qserver.GameServer.Network;
using TNL.Entities;
using Qserver.Util;
using TNL.Utils;

namespace Qserver.GameServer.Qpang
{
    public abstract class GameNetEvent : NetEvent
    {
        public GameNetId Id;
        public GameConnection GameConnection;

        public GameNetEvent() : base(GuaranteeType.GuaranteedOrdered, EventDirection.DirUnset) { }

        public GameNetEvent(GameNetId gameNetId, GuaranteeType guaranteeType, EventDirection eventDirection) : base(guaranteeType, eventDirection)
        {
            Id = gameNetId;
        }

        public override void NotifyPosted(EventConnection e) { }
        public override void NotifyDelivered(EventConnection e, bool madeId) { }

        public void Post(EventConnection ps)
        {
            this.GameConnection = (GameConnection)ps;
            Game.Instance.RoomServer.HandleEvent(this);
        }

        public virtual void Handle(GameConnection conn, Player player)
        {
            Log.Message(LogType.ERROR, $"GameNetEvent Unhandeled {Id}!");
        }
    }
}
