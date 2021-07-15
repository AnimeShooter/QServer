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
    }

    public class ChannelManager
    {
        private ChannelsRepository _channelsRepository;
        private Dictionary<uint, Channel> _channels;

        public ChannelManager()
        {
            Log.Message(LogType.MISC, "Loading Channels from database...");
            this._channelsRepository = new ChannelsRepository(DatabaseManager.MySqlFactory);
            this._channels = new Dictionary<uint, Channel>();
            foreach(var c in this._channelsRepository.GetChannels().Result)
            {
                Log.Message(LogType.DUMP, $"[{c.Name}] Max:{c.MaxLevel} @ {c.IP}\n");
                this._channels.Add(c.Id, c);
            }
            Log.Message(LogType.MISC, $"{this._channels.Count} Channels loaded!");
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
