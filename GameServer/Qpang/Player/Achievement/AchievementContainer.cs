using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class AchievementContainer
    {
        private List<uint> _achievements;
        private List<uint> _recentAchievements;

        public List<uint> List
        {
            get { return this._achievements; }
        }

        public List<uint> ListRecent
        {
            get { return this._recentAchievements; }
        }

        private uint _playerId;
        private object _lock;

        public AchievementContainer(uint playerId)
        {
            this._lock = new object();
            this._playerId = playerId;
            this._achievements = new List<uint>();
            this._recentAchievements = new List<uint>();
            // TODO: database
        }

        public void Unlock(uint achievementId)
        {
            lock(this._lock)
            {
                if (this._achievements.Count == 0 || achievementId != this._achievements[this._achievements.Count])
                    return;

                // TODO: update database

                this._achievements.Add(achievementId);
                this._recentAchievements.Add(achievementId);
            }
        }

        public void ResetRecent()
        {
            this._recentAchievements.Clear();
        }

    }
}
