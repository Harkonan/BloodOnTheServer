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
        private string _theme;

        public VoteController(SessionContext context)
        {
            _context = context;
        }



        [Route("/vote/{id}/{voteSession}")]
        public IActionResult Index( int id, string voteSession)
        {
            if (Request.Cookies.ContainsKey("theme"))
            {
                _theme = Request.Cookies["theme"];
            }
            else
            {
                _theme = "light";
            }
            ViewBag.theme = _theme;

            Guid PlayerID = GetOrSetPlayerID(voteSession);

            if (string.IsNullOrEmpty(voteSession))
            {
                return RedirectToAction("Create");
            }
            else
            {
                _context.Sessions.Where(x => x.SessionId == voteSession).FirstOrDefault().LastUsed = DateTime.Now;
                _context.SaveChanges();
            }

            //if (id > 0)
            //{
            //    var Players = _context.Players.Where(x => x.Session.SessionId == voteSession).ToList();

            //    //remove all instances of this player having a seat already
            //    if (Players.Any(x => x.PlayerID == PlayerID))
            //    {
            //        _context.RemoveRange(Players.Where(x => x.PlayerID == PlayerID && x.PlayerSeat != id));
            //    }

            //    _context.SaveChanges();

            //    Players = _context.Players.Where(x => x.Session.SessionId == voteSession).ToList();
            //    //if this player is not already added in as using the current seat, add them
            //    if (!Players.Any(x => x.PlayerID == PlayerID && x.PlayerSeat == id))
            //    {
            //        Player P = new Player()
            //        {
            //            PlayerID = PlayerID,
            //            PlayerSeat = id,
            //            Session = _context.Sessions.Where(x => x.SessionId == voteSession).FirstOrDefault()
            //        };
            //        _context.Players.Add(P);
            //        _context.Entry(P).State = EntityState.Added;
            //        _context.SaveChanges();
            //    }
            //}

            var numberOfVoters = _context.Sessions.Where(x => x.SessionId == voteSession).FirstOrDefault().Seats;


            if (id != 100 && id != 0)
            {
                SetCookie(voteSession.ToString() + "_Seat", id.ToString(), null, true);
            }

            VotePageInfo Page = new VotePageInfo()
            {
                PlayerId = PlayerID.ToString(),
                MyVoteId = id,
                NumberOfVotes = numberOfVoters,
                VoteSession = voteSession,
            };
            _context.SaveChanges();
            return View(Page);
        }

        [Route("/vote/spectate/{voteSession}")]
        public IActionResult Spectate(string voteSession)
        {
            return RedirectToAction("index", new { id = 100, voteSession });
        }

        [Route("/vote/lobby/{voteSession}")]
        public IActionResult Lobby(string voteSession)
        {
            ViewBag.Session = voteSession;
            return View();
        }

        //https://localhost:44388/vote/Join/76c2c226bfe
        [Route("/vote/join/{voteSession}")]
        public IActionResult Join(string voteSession)
        {
            int FirstEmptySeat = 21;

            if (Request.Cookies.ContainsKey(voteSession.ToString() + "_seat"))
            {
                FirstEmptySeat = Convert.ToInt32(Request.Cookies[voteSession.ToString() + "_seat"]);
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
            }

            SetCookie(voteSession.ToString() + "_Seat", FirstEmptySeat.ToString(), null, true);
            return RedirectToAction("index", new { id = FirstEmptySeat, voteSession });
        }



        [Route("/vote/create")]
        public IActionResult Create()
        {
            string voteSession = GetIdentifier();
            _context.Sessions.Add(new Session()
            {
                SessionId = voteSession,
                LastUsed = DateTime.Now,
                Seats = 7
            });
            _context.SaveChanges();

            return RedirectToAction("index", new { id = 0, voteSession });
        }

        public IActionResult Help()
        {
            if (Request.Cookies.ContainsKey("theme"))
            {
                _theme = Request.Cookies["theme"];
            }
            else
            {
                _theme = "light";
            }
            ViewBag.theme = _theme;
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

        private string GetIdentifier()
        {
            var ticks = new DateTime(2020, 5, 4).Ticks;
            var ans = DateTime.Now.Ticks - ticks;
            return ans.ToString("x");
        }

        private Guid GetOrSetPlayerID(string SessionId)
        {
            Guid PlayerID = new Guid();
            if (Request.Cookies.ContainsKey(SessionId + "_PlayerID"))
            {
                PlayerID = new Guid(Request.Cookies[SessionId + "_PlayerID"]);
            }
            else
            {
                PlayerID = Guid.NewGuid();
                SetCookie(SessionId + "_PlayerID", PlayerID.ToString(), null, true);
            }
            return PlayerID;
        }

    }


}