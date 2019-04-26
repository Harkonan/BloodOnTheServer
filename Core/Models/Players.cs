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
        public int DayOfDeath { get; set; }
        public bool HasVote { get; set; }
        public Role Role { get; set; }

        public Player()
        {
            Id = Guid.NewGuid();
            IsAlive = true;
            HasVote = true;
            CauseOfDeath = CauseOfDeath.NotDead;
        }

        /// <summary>
        /// Kills a player and assigned all information needed to process a dead player for night visit logic
        /// </summary>
        /// <param name="causeOfDeath">The way the player was killed.</param>
        /// <param name="dayOfDeath">The Day number the player was killed on.</param>
        public void KillPlayer(CauseOfDeath causeOfDeath, int dayOfDeath)
        {
            IsAlive = false;
            CauseOfDeath = causeOfDeath;
            DayOfDeath = dayOfDeath;
        }

        /// <summary>
        /// Assignes a new role to a player and sets a night visit to tell them of their new role.
        /// </summary>
        /// <param name="newRole">Role to assign</param>
        public void ChangeRole(Role newRole)
        {
            Role = newRole;
            Role.NightVisitMarker = new NightVisit {
                PreSetup = false
            };
        }
    }

    public class Players
    {
        public List<Player> PlayersList { get; set; }

        public Players()
        {
            PlayersList = new List<Player>();
        }

        /// <summary>
        /// Kills a player in the Player list by their name. Returns a boolean to confirm the kill.
        /// </summary>
        /// <param name="PlayerName">Player name string (exact match needed)</param>
        /// <param name="_CauseOfDeath">Cause of the players death</param>
        /// <returns></returns>
        public bool KillPlayer(string PlayerName, CauseOfDeath _CauseOfDeath)
        {
            if (PlayersList.Any(x => x.Name.Trim().ToLower() == PlayerName.Trim().ToLower()))
            {
                var Player = PlayersList.Where(x => x.Name.Trim().ToLower() == PlayerName.Trim().ToLower()).First();
                Player.IsAlive = false;
                Player.CauseOfDeath = _CauseOfDeath;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns a list of players who have a night visit reminder set.
        /// </summary>
        /// <returns>List<Player></returns>
        public List<Player> GetPlayersWithNightVisits()
        {
            return PlayersList.Where(p => p.Role.NightVisitMarker != null).ToList();
        }

        /// <summary>
        /// Returns a list of players who have night visit reminders assigned with a presetup tag.
        /// </summary>
        /// <returns>List<Player></returns>
        public List<Player> GetPlayersWithNightVisitsToPreSetup()
        {
            return PlayersList.Where(p => p.Role.NightVisitMarker != null && p.Role.NightVisitMarker.PreSetup).ToList();
        }

    }

    public enum CauseOfDeath { NotDead, Execution, Exile, Demon, Other }

}
