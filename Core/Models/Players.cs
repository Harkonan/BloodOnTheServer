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
        public CauseOfDeath CauseOfDeath { get; set; }
        public bool HasVote { get; set; }
        public Role Role { get; set; }

        public Player()
        {
            Id = Guid.NewGuid();
            IsAlive = true;
            HasVote = true;
            CauseOfDeath = CauseOfDeath.NotDead;
        }

        public void KillPlayer(CauseOfDeath _CauseOfDeath)
        {
            if ((Role.Name != "Soldier" || (Role.IsDrunk || Role.IsPoisoned)) && _CauseOfDeath == CauseOfDeath.Demon)
            {
                IsAlive = false;
                CauseOfDeath = _CauseOfDeath;
            }
        }
    }

    public class Players {
        public List<Player> PlayersList { get; set; }

        public Players()
        {
            PlayersList = new List<Player>();
        }

        public bool KillPlayer(string PlayerName, CauseOfDeath _CauseOfDeath)
        {
            if(PlayersList.Any(x => x.Name == PlayerName))
            {
                var Player = PlayersList.Where(x => x.Name == PlayerName).First();
                Player.IsAlive = false;
                Player.CauseOfDeath = _CauseOfDeath;
            }
            return false;
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

    public enum CauseOfDeath { NotDead, Execution, Exile, Demon, Other }

}
