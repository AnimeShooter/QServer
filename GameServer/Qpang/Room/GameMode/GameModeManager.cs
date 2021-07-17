using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class GameModeManager
    {
        private Dictionary<byte, GameMode> _gameModes = new Dictionary<byte, GameMode>()
        {
            { 1, new DeathMatch() },
            { 2, new TeamDeathMatch() },
            { 3, new ProtectTheEssence() },
            { 4, new VIP() },

            { 5, null }, // Practice
            { 8, null }, // Public Enemy
            { 9, null }, // PvE (STO)
        };

        public GameMode GetGameMode(byte mode)
        {
            if (_gameModes.ContainsKey(mode))
                return _gameModes[mode];
            return _gameModes[0];
        }
    }
}
