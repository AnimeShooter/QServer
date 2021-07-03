﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class RoomSessionPlayer
    {
        private PlayerEffectManager _effectManager;
        private PlayerWeaponManager _weaponManager;
        private RoomSkillManager _skillManager;
        private PlayerEntityManager _entityManager;

        private Position _position;

        private DateTime _joinTime;
        private DateTime _startTime;
        private DateTime _invincibleRemovalTime;
        private DateTime _respawnTime;

        private bool _isPlaying; // otherwise waiting
        private bool isRespawning;
        private bool _isSpectating;

        private ushort _character = 343;
        private byte _team = 0;
        private ushort _health = 100;
        private ushort _baseHealth = 100;
        private ushort _bonusHealth = 100;

        private uint[] _armor = new uint[9];

        private object _bulletLock;

        private bool _hasQuickRevive;
        private bool _isInvincible;

        private ushort _expRate;
        private ushort _donRate;

        private ushort _exp;
        private ushort _don;

        private ushort _highestStreak;
        private ushort _streak;
        private ushort _kills;
        private ushort _deaths;
        private ushort _score;
        private uint _playTime;
        private uint _highestMultiKill;
        private uint _eventItemPickUps;

        private GameConnection _conn;
        private RoomSession _roomSession;

        public uint HighestMultiKill
        {
            get { return this._highestMultiKill; }
            set { this._highestMultiKill = value; }
        }

        public RoomSessionPlayer(GameConnection conn, RoomSession roomSession, byte team)
        {
            this._conn = conn;
            this._roomSession = roomSession;
            this._team = team;
            this._isPlaying = false;
            this._isInvincible = false;
            this._isSpectating = false;
            this._streak = 0;
            this._kills = 0;
            this._deaths = 0;
            this._score = 0;
            this._exp = 0;
            this._expRate = 0;
            this._don = 0;
            this._donRate = 0;
            this._playTime = 0;
            this._highestStreak = 0;
            this._highestMultiKill = 0;
            this._eventItemPickUps = 0;
        }
    }
}