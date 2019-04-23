using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Core.Models
{
    public class Player
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsAlive { get; set; }
        public bool HasVote { get; set; }
        public Role Role { get; set; }

        public Player()
        {
            Id = Guid.NewGuid();
            IsAlive = true;
            HasVote = true;
        }
    }

    public class Players {
        public List<Player> PlayersList { get; set; }

        public Players()
        {
            PlayersList = new List<Player>();
        }

        public List<Player> GetPlayersWithNightVisits()
        {
            return PlayersList.Where(p => p.Role.NightVisitMarker != null).ToList();
        }

        public List<Player> GetPlayersWithNightVisitsToPreSetup()
        {
            return PlayersList.Where(p => p.Role.NightVisitMarker != null && p.Role.NightVisitMarker.PreSetup).ToList();
        }

    }

}
