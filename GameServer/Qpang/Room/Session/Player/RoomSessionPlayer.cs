using System;
using System.Collections.Generic;
using System.Text;
using Qserver.GameServer.Qpang.Room.Session.Player.Effect;
using Qserver.GameServer.Qpang.Room.Session.Player.Entity;
using Qserver.GameServer.Qpang.Room.Session.Player.Skill;
using Qserver.GameServer.Qpang.Room.Session.Player.Weapon;

namespace Qserver.GameServer.Qpang.Room.Session.Player
{
    public class RoomSessionPlayer
    {
        private PlayerEffectManager _effectManager;
        private PlayerWeaponManager _weaponManager;
        private PlayerSkillManager _skillManager;
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

        private mutex _bulletMx;

        private bool _hasQuickRevive;
        private bool _isInvincible;

        private ushort _expRate;
        private ushort _donRate;

        private ushort _exp;
        private ushort _don;

        private ushort m_highestStreak;
        private ushort m_streak;
        private ushort m_kills;
        private ushort m_deaths;
        private ushort m_score;
        private uint m_playTime;
        private uint m_highestMultiKill;
        private uint m_eventItemPickUps;

        // private conn
        private RoomSession _roomSession;
    }
}
