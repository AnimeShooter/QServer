using System;
using System.Collections.Generic;
using System.Text;
using Qserver.GameServer.Database;
using Qserver.GameServer.Database.Repositories;

namespace Qserver.GameServer.Qpang
{
    public struct Channel
    {
        public ushort Id;
        public string Name;
        public byte MinLevel;
        public byte MaxLevel;
        public ushort MaxPlayers;
        public ushort CurrPlayers;
        public string IP;
    }

    public class ChannelManager
    {
        private ChannelsRepository _channelsRepository;
        private Dictionary<uint, Channel> _channels;

        public ChannelManager()
        {
            this._channelsRepository = new ChannelsRepository(DatabaseManager.MySqlFactory);
            this._channels = new Dictionary<uint, Channel>();
            foreach(var c in this._channelsRepository.GetChannels().Result)
            {
                this._channels.Add(c.Id, c);
            }
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
