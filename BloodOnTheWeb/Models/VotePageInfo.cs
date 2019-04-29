using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BloodOnTheWeb.Models
{
    public class VotePageInfo
    {
        public int NumberOfVotes { get; set; }
        public int MyVoteId { get; set; }
        public Guid VoteSession { get; set; }
    }
}
