using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class Role
    {
        public Guid Id { get; set; }
        public int? NightPriority { get; set; }
        public int? FirstNightPriority { get; set; }
        public string Name { get; set; }
        public bool IsDrunk { get; set; }
        public bool IsPoisoned { get; set; }
        public NightVisit NightVisitMarker { get; set; }
        public Team Team { get; set; }
        public RoleType Type { get; set; }
        public string RoleText { get; set; }

        public Role()
        {
            SetupStandardValues();
        }

        public Role(string Role)
        {
            SetupStandardValues();
        }

        private void SetupStandardValues()
        {
            Id = Guid.NewGuid();
            IsDrunk = false;
            IsPoisoned = false;
        }


    }

    public enum Team { Good, Evil }
    public enum RoleType { Outsider, Townsfolk, Minion, Demon }

}
