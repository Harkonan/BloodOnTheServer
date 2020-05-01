using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BloodOnTheWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        
        public IActionResult Index(int numberOfVoters, int id,  Guid voteSession)
        {
            if (voteSession == Guid.Empty)
            {
                voteSession = Guid.NewGuid();

                _context.Sessions.Add(new Session()
                {
                    SessionId = voteSession
                });
                _context.SaveChanges();
            }

            if (id > 0)
            {
                var DbSession = _context.Sessions.Where(x => x.SessionId == voteSession).FirstOrDefault();
                Player P = new Player()
                {
                    PlayerID = Guid.NewGuid(),
                    PlayerSeat = id,
                    Session = DbSession
                };

                _context.Players.Add(P);
                _context.SaveChanges();
            }

         
            
            if (numberOfVoters == 0)
            {
                numberOfVoters = 7;
            }

            if (id != 100)
            {
                SetCookie(voteSession.ToString() + "_Seat", id.ToString(), null, true);
            }
            

            VotePageInfo Page = new VotePageInfo()
            {
                MyVoteId = id,
                NumberOfVotes = numberOfVoters,
                VoteSession = voteSession
            };

            return View(Page);
        }

        public IActionResult Spectate(Guid voteSession)
        {
            return RedirectToAction("index", new { id = 100, numberOfVoters = 7, voteSession = voteSession });
        }


        public IActionResult Join(Guid voteSession)
        {
            int FirstEmptySeat = 21;

            if (Request.Cookies.ContainsKey(voteSession.ToString() + "_seat"))
            {
                FirstEmptySeat =Convert.ToInt32(Request.Cookies[voteSession.ToString() + "_seat"]);
            }
            else
            {

                var DbSession = _context.Sessions.Include("Players").Where(x => x.SessionId == voteSession).FirstOrDefault();
                var DbSessionPlayers = DbSession.Players.ToList();

                

                for (int i = 1; i <= 20; i++)
                {
                    if (!DbSessionPlayers.Any(x => x.PlayerSeat == i))
                    {
                        FirstEmptySeat = i;
                        break;
                    }
                }

                SetCookie(voteSession.ToString() + "_Seat", FirstEmptySeat.ToString(), null, true);
            }
            return RedirectToAction("index", new { id = FirstEmptySeat, numberOfVoters = 7, voteSession = voteSession });
        }



        public IActionResult Help()
        {
            return View();
        }

        public void SetCookie(string key, string value, int? expireTime, bool essential = false)
        {
            CookieOptions option = new CookieOptions();
            option.IsEssential = essential;

            if (expireTime.HasValue)
                option.Expires = DateTime.Now.AddMinutes(expireTime.Value);
            else
                option.Expires = DateTime.Now.AddDays(1);

            Response.Cookies.Append(key, value, option);
        }

    }


}