using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class Role
    {
        public Guid Id { get; set; }
        public int NightPriority { get; set; }
        public int FirstNightPriority { get; set; }
        public string Name { get; set; }
        public bool IsDrunk { get; set; }
        public bool IsPoisoned { get; set; }
        public bool HasVote { get; set; }
        public List<Reminder> Reminders { get; set; }
        public Team Team { get; set; }
        public RoleType Type { get; set; }
    }

    public enum Team { Good, Evil }
    public enum RoleType { Outside, Townsfolk, Minion, Demon }

}
