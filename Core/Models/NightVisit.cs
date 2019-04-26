using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class NightVisit
    {
        public Guid Id { get; set; }
        public bool PreSetup { get; set; }


        public NightVisit()
        {
            Id = Guid.NewGuid();
            PreSetup = false;
        }

        public NightVisit(bool preSetup)
        {
            Id = Guid.NewGuid();
            PreSetup = preSetup;
        }
    }
}
