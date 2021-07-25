using System;
using System.Collections.Generic;
using System.Text;
using Qserver.GameServer.Database;
using Qserver.GameServer.Database.Repositories;
using Qserver.Util;

namespace Qserver.GameServer.Qpang
{
    public struct Channel
    {
        public ushort Id;
        public string Name;
        public byte MinLevel;
        public byte MaxLevel;
        public ushort MaxPlayers;
        public ushort MinRank;
        public string IP;
        public ushort CurrPlayers;
        public byte TestRealm;
    }

    public class ChannelManager
    {
        private Dictionary<uint, Channel> _channels;

        public ChannelManager()
        {
            this._channels = new Dictionary<uint, Channel>();
            var channels = Game.Instance.ChannelsRepository.GetChannels().Result;
            foreach(var c in channels)
            {
                Log.Message(LogType.DUMP, $"{$"[{c.Name + (c.TestRealm == 1 ? "*" : "")}]".PadRight(28)} MaxP:{c.MaxPlayers.ToString().PadLeft(3)} @ {c.IP}\n");
                this._channels.Add(c.Id, c);
            }
            Log.Message(LogType.MISC, $"ChannelManager loaded {this._channels.Count} from the database!");
        }

        public List<Channel> List()
        {
            List<Channel> channels = new List<Channel>();
            foreach (var c in this._channels)
                channels.Add(c.Value);
            return channels;
        }

        public Channel GetChannel(uint channelId)
        {
            if (this._channels.ContainsKey(channelId))
                return this._channels[channelId];
            return new Channel(); // default?
        }
    }
}
