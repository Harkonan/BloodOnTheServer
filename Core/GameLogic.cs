using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class GameLogic
    {
        public List<Player> Players { get; set; }

        public GameResult CheckForWin()
        {
            if (Players.Where(x => x.Role.Type == RoleType.Demon).Count() == 0)
            {
                return new GameResult
                {
                    GameWon = true,
                    WinText = "Demon has been killed",
                    WonBy = Team.Good
                };

            } else if (Players.Where(x => x.IsAlive).Count() <= 2)
            {
                return new GameResult {
                    GameWon = true,
                    WinText = "Only 2 players left alive",
                    WonBy = Team.Evil
                };
            }

            return new GameResult { GameWon = false };
        }


    }
}
