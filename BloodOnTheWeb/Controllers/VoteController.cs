using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BloodOnTheWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BloodOnTheWeb.Controllers
{
    public class VoteController : Controller
    {
        private readonly SessionContext _context;

        public VoteController(SessionContext context)
        {
            _context = context;
        }

        public IActionResult Index(int id, int numberOfVoters, Guid voteSession)
        {
            if (voteSession == Guid.Empty)
            {
                voteSession = Guid.NewGuid();
            }

            if (numberOfVoters == 0)
            {
                numberOfVoters = 7;
            }

            VotePageInfo Page = new VotePageInfo()
            {
                MyVoteId = id,
                NumberOfVotes = numberOfVoters,
                VoteSession = voteSession
            };

            return View(Page);
        }


        public IActionResult Join(int numberOfVoters, Guid voteSession)
        {
            _context.Sessions.Add(new Session()
            {
                SessionId = Guid.NewGuid()
            });
            _context.SaveChanges();

            return View("Index");
        }



        public IActionResult Help()
        {
            return View();
        }
    }


}