using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BloodOnTheWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BloodOnTheWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoteAPIController : ControllerBase
    {

        private readonly SessionContext _context;

        public VoteAPIController(SessionContext context)
        {
            _context = context;
        }

        [Route("SetSeats")]
        public HttpResponseMessage SetSessionSeats(string SessionId, int NewSeats)
        {
            _context.Sessions.Find(SessionId).Seats = NewSeats;
            _context.SaveChanges();
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [Route("GetSeats")]
        public int SetSessionSeats(string SessionId)
        {
            return _context.Sessions.Find(SessionId).Seats;
        }
    }
}