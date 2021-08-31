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

        // helpers
        public void WriteWString(BitStream bitSteam, string str, uint maxLen = 0)
        {
            if (maxLen == 0)
                maxLen = (uint)str.Length;

            byte[] strBuffer = new byte[(uint)(maxLen * 2 + 2)];
            ByteBuffer byteBuffer = new ByteBuffer((uint)strBuffer.Length);

            //byte[] data = Encoding.Unicode.GetBytes(str);
            //for (int i = 0; i < str.Length; i++)
            //{
            //    strBuffer[i] = data[i];
            //    strBuffer[i + 1] = data[i + 1];
            //}

            // NOTE: proper support?
            for (int i = 0; i < str.Length; i++)
            {
                byte[] wchar = BitConverter.GetBytes(str[i]);
                strBuffer[i * 2] = wchar[0];
                strBuffer[i * 2 + 1] = wchar[1];
            }
            
            byteBuffer.SetBuffer(strBuffer, (uint)strBuffer.Length);
            bitSteam.Write(byteBuffer);
        }

        public string ByteBufferToString(ByteBuffer bb, bool wchar = true)
        {
            byte[] data = bb.GetBuffer();
            string result = "";
            for(int i = 0; i < data.Length; i+= (wchar ? 2 : 1))
            {
                if (data[i] != 0x00)
                    result += (char)data[i];
            }
            return result;
        }
    }

}

