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

        public void AddFirstNightReminders()
        {
            SetFirstNightReminder("Chef");
            SetFirstNightReminder("Empath");
            SetFirstNightReminder("Fortune Teller");
            SetFirstNightReminder("Investigator", true);
            SetFirstNightReminder("Librarian", true);
        }

        public void AddNightReminders()
        {
            SetStandardNightReminders("Empath");
            SetStandardNightReminders("Fortune Teller");
            SetStandardNightReminders("Imp");
            SetStandardNightReminders("Scarlet Woman", true, Players.Any(x => x.Role.Type == RoleType.Demon && !x.IsAlive));
        }

        private void SetFirstNightReminder(string Role, bool PreSetup = false)
        {
            if (Players.Any(x => x.Role.Name == Role))
            {
                var player = Players.Where(x => x.Role.Name == Role).First();
                player.Role.NightVisitMarker = new NightVisit(PreSetup);
            }
        }

        private void SetStandardNightReminders(string Role, bool PreSetup = false, bool AdditionalLogic = true)
        {
            if (Players.Any(x => x.Role.Name == Role && x.IsAlive && AdditionalLogic))
            {
                var player = Players.Where(x => x.Role.Name == Role).First();
                player.Role.NightVisitMarker = new NightVisit(PreSetup);
            }
        }

    }
}
