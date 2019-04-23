using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class GameResult
    {
        public bool GameWon { get; set; }
        public String WinText { get; set; }
        public Team WonBy { get; set; }
    }
}
