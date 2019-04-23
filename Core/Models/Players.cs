using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class Player
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsAlive { get; set; }
        public int MyProperty { get; set; }
        public Role Role { get; set; }
    }
}
