using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public enum GameModeName : byte
    {
        DeathMatch = 1,
        TeamDeathMatch,
        ProtectTheEssence,
        VIP,

        Practice = 5,
        PublicEnemy = 8,
        PvE = 9
    }

    public class GameModeManager
    {
        private Dictionary<GameMode.Mode, GameMode> _gameModes = new Dictionary<GameMode.Mode, GameMode>()
        {
            { GameMode.Mode.DM, new DeathMatch() },
            { GameMode.Mode.TDM, new TeamDeathMatch() },
            { GameMode.Mode.PTE, new ProtectTheEssence() },
            { GameMode.Mode.VIP, new VIP() },

            { GameMode.Mode.PRACTICE, null }, // Practice
            { GameMode.Mode.PREY, new PublicEnemy() }, // Public Enemy
            { GameMode.Mode.PVE, new PvE() }, // PvE (STO)
        };

        public GameMode Get(GameMode.Mode mode)
        {
            if (_gameModes.ContainsKey(mode))
                return _gameModes[mode];
            return _gameModes[0];
        }
    }
}
