using BloodOnTheWeb.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace BloodOnTheWeb.Hubs
{
    
    public class Clocktower : Hub
    {

        private readonly SessionContext _context;

        public Clocktower(SessionContext context)
        {
            _context = context;
        }

        public async Task ClientToServerVote(string voterId, string voterName, string newVote, string health, string vote_status, string afk_status, Guid session)
        {
            await Clients.Group(session.ToString()).SendAsync("ServerToClientVote", voterId, voterName, newVote, vote_status, health, afk_status);
        }
            
        public async Task ClientRequestsLatest(Guid session)
        {
            await Clients.Group(session.ToString()).SendAsync("GetCurrentClientVote");
        }

        public async Task AdminUpdateRecord(Guid session, string log)
        {
            await Clients.Group(session.ToString()).SendAsync("UpdateLog", log);
        }

        public async Task AdminTriggerReadyCheck(Guid session)
        {
            await Clients.Group(session.ToString()).SendAsync("GetReadyResponse");
        }

        public async Task ClientReadyResponse(Guid session, string voterId){
            await Clients.Group(session.ToString()).SendAsync("PlayerReady", voterId);
        }

        public async Task AdminSendStartTimer(int timePerUser, string type, string start, Guid session)
        {
            await Clients.Group(session.ToString()).SendAsync("StartTimer", timePerUser, type, start);
        }

        public async Task AdminSendNewPlayerCount(int newPlayerNumber, Guid session)
        {
            await Clients.Group(session.ToString()).SendAsync("ChangePlayerNumber", newPlayerNumber);
        }

        public async Task RequestPlayerNumber(Guid session)
        {
            await Clients.Group(session.ToString()).SendAsync("PlayerRequestPlayerNumber");
        }

        public async Task AdminSwapPlayers(int voterOne, int voterTwo, Guid session)
        {
            await Clients.Group(session.ToString()).SendAsync("SwapPlayers", voterOne, voterTwo);
        }

        public async Task JoinSession(Guid session, int playerSeat)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, session.ToString());
        }
    }
}
