using System;
using System.Collections.Generic;
using System.Text;
using Qserver.GameServer.Qpang.Room.GameMode;
using Qserver.GameServer.Qpang.Room.GameMode.Modes;
using Qserver.GameServer.Qpang.Room.Session.GameItem;
using Qserver.GameServer.Qpang.Room.Session.Player;
using Qserver.GameServer.Qpang.Room.Session.Player.Skill;

namespace Qserver.GameServer.Qpang.Room.Session
{
    public class RoomSession
    {
        private Room _room;

        private bool _isFinished;
        private bool _isPoints;

        private uint _goal;
        private uint _bluePoints;
        private uint _yellowPoints;

        private DateTime _startTime;
        private DateTime _endTime;
        private DateTime _lastTickTime;

        private DateTime _essenceDropTime;
        private bool _isEssenceReset;

        private GameMode.GameMode gameMode;
        private GameItemManager _itemManager;
        private RoomSkillManager _skillManager;

        private object _playerlock; // NOTE: multiple locks?
        private Dictionary<uint, RoomSessionPlayer> _players;

        private object _leaverLock;
        private List<RoomSessionPlayer> _leavers;

        private RoomSessionPlayer _essenceHolder;
        private Spawn essencePosition = { 0, 0, 0 };

        private RoomSessionPlayer _blueVIP;
        private RoomSessionPlayer _nexBlueVIP;
        private DateTime _blueVIPSetTime;
        private RoomSessionPlayer _yellowVIP;
        private RoomSessionPlayer _nexYellowVIP;
        private DateTime _yellowVIPSetTime;

    }
}
