using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core
{
    public class NightVisitLogic
    {
        private List<Player> Players { get; set; }

        public NightVisitLogic(List<Player> _Players)
        {
            Players = _Players;
        }

        public void ClearNightVisits()
        {
            foreach(var Player in Players.Where(x => x.Role.NightVisitMarker != null))
            {
                Player.Role.NightVisitMarker = null;
            }
        }

        public void AddFirstNightVisits()
        {
            SetFirstNightVisit("Poisoner");
            SetFirstNightVisit("Washerwoman", true);
            SetFirstNightVisit("Librarian", true);
            SetFirstNightVisit("Investigator", true);
            SetFirstNightVisit("Chef");
            SetFirstNightVisit("Empath");
            SetFirstNightVisit("Fortune Teller", true);
            SetFirstNightVisit("Butler");
            SetFirstNightVisit("Spy");
        }

        public void RefreshNightVisits()
        {
            ClearNightVisits();
            AddNightVisits();
        }

        public void AddNightVisits()
        {
            SetStandardNightVisit("Poisoner");
            SetStandardNightVisit("Monk");
            
            //wont work until we impliment death system
            SetStandardNightVisit("Scarlet Woman", true, Players.Any(x => x.Role.Type == RoleType.Demon && !x.IsAlive)); 
            SetStandardNightVisit("Imp");

            //wont work until we impliment death system
            SetRevenKeeperVisit(); 
            SetStandardNightVisit("Empath");
            SetStandardNightVisit("Fortune Teller");
            SetStandardNightVisit("Butler");
            SetUndertakerVisit();
            SetStandardNightVisit("Spy");

        }

        private void SetFirstNightVisit(string Role, bool PreSetup = false)
        {
            if (Players.Any(x => x.Role.Name == Role))
            {
                var player = Players.Where(x => x.Role.Name == Role).First();
                player.Role.NightVisitMarker = new NightVisit(PreSetup);
            }
        }

        private void SetStandardNightVisit(string Role, bool PreSetup = false, bool AdditionalLogic = true)
        {
            if (Players.Any(x => x.Role.Name == Role && x.IsAlive && AdditionalLogic))
            {
                var player = Players.Where(x => x.Role.Name == Role).First();
                player.Role.NightVisitMarker = new NightVisit(PreSetup);
            }
        }

        private void SetUndertakerVisit()
        {
            bool AnyExecutedPlayersNotVisitedYet = Players.Any(p => !p.IsAlive && p.CauseOfDeath == CauseOfDeath.Execution && !p.Role.VisitedByTheUndertaker);

            if (Players.Any(x => x.Role.Name == "Undertaker" && x.IsAlive && AnyExecutedPlayersNotVisitedYet))
            {
                var player = Players.Where(x => x.Role.Name == "Undertaker").First();
                player.Role.NightVisitMarker = new NightVisit();
                
            }
        }

        private bool RavenKeeperSkillUsed = false;
        private void SetRevenKeeperVisit()
        {
            if (Players.Any(x => x.Role.Name == "Ravenkeeper" && !x.IsAlive && x.CauseOfDeath == CauseOfDeath.Demon))
            {
                var player = Players.Where(x => x.Role.Name == "Ravenkeeper").First();
                player.Role.NightVisitMarker = new NightVisit();
            }
        }

    }
}
