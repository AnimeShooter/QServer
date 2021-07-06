using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
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

        private GameMode gameMode;
        private GameItemManager _itemManager;
        private RoomSkillManager _skillManager;

        private object _playerlock; // NOTE: multiple locks?
        private Dictionary<uint, RoomSessionPlayer> _players;

        private object _leaverLock;
        private List<RoomSessionPlayer> _leavers;

        private RoomSessionPlayer _essenceHolder;
        private Spawn essencePosition = new Spawn(){ X=0, Y=0, Z=0 };

        private RoomSessionPlayer _blueVIP;
        private RoomSessionPlayer _nexBlueVIP;
        private DateTime _blueVIPSetTime;
        private RoomSessionPlayer _yellowVIP;
        private RoomSessionPlayer _nexYellowVIP;
        private DateTime _yellowVIPSetTime;

        public void RemovePlayer(uint id)
        {

        }
    }
}
