using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BloodOnTheWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BloodOnTheWeb.Controllers
{
    public class VoteController : Controller
    {
        public IActionResult Index(int id, int numberOfVoters, Guid voteSession)
        {
            if (voteSession == Guid.Empty)
            {
                voteSession = Guid.NewGuid();
            }

            VotePageInfo Page = new VotePageInfo()
            {
                MyVoteId = id,
                NumberOfVotes = numberOfVoters,
                VoteSession = voteSession
            };  

            return View(Page);
        }
    }
}