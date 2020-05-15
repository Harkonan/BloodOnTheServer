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
        private string ConnectedUID;

        public Clocktower(SessionContext context)
        {
            _context = context;
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            //remove player from DB so seat can be re allocated
            //need either their seat or their player ID which is currently transient
            Guid PlayerUID = new Guid(Context.Items["UID"].ToString());
            var Player = _context.Players.Where(x => x.PlayerID == PlayerUID);
            _context.RemoveRange(Player);
            _context.SaveChanges();
            await ClientToServerVote("vote_"+Context.Items["Seat"].ToString(), "voter_" + Context.Items["Seat"].ToString(), "True", "alive", "false", "free", "false", Context.Items["Session"].ToString());
            await base.OnDisconnectedAsync(exception);
        }

        public async Task AdminPing(string session, string pingId)
        {
            await Clients.Group(session.ToString()).SendAsync("AdminRequestPong", pingId);
        }

        public async Task ClientPong(string session, string pingId, int MySeat, string UID)
        {
            await Clients.Group(session.ToString()).SendAsync("AdminRecievdPong", pingId, MySeat, UID);
        }

        public async Task ResolveDuplicates(string session, string UID)
        {
            await Clients.Group(session.ToString()).SendAsync("ClientResolveDuplicates", UID);
        }

        public async Task ClientToServerVote(string voterId, string voterName, string newVote, string health, string traveller, string vote_status, string afk_status, string session)
        {
            await Clients.Group(session.ToString()).SendAsync("ServerToClientVote", voterId, voterName, newVote, vote_status, health, traveller, afk_status);
        }

        public async Task ClientRequestsLatest(string session)
        {
            await Clients.Group(session.ToString()).SendAsync("GetCurrentClientVote");
        }

        public async Task AdminUpdateRecord(string session, string log)
        {
            await Clients.Group(session.ToString()).SendAsync("UpdateLog", log);
        }

        public async Task AdminTriggerReadyCheck(string session)
        {
            await Clients.Group(session.ToString()).SendAsync("GetReadyResponse");
        }

        public async Task ClientReadyResponse(string session, string voterId)
        {
            await Clients.Group(session.ToString()).SendAsync("PlayerReady", voterId);
        }

        public async Task AdminSendStartTimer(int timePerUser, string type, string start, string session)
        {
            await Clients.Group(session.ToString()).SendAsync("StartTimer", timePerUser, type, start);
        }

        public async Task AdminTriggerCheckPlayerCount(string session)
        {
            await Clients.Group(session.ToString()).SendAsync("CheckPlayerNumber");
        }

        public async Task AdminSwapPlayers(int voterOne, int voterTwo, string session)
        {
            await Clients.Group(session.ToString()).SendAsync("SwapPlayers", voterOne, voterTwo);
        }

        public async Task AdminMovePlayer(int oldSeat, int newSeat, string session)
        {
            await Clients.Group(session.ToString()).SendAsync("ChangeSeat", oldSeat, newSeat);
        }

        public async Task JoinSession(string session, int playerSeat, string MyUID)
        {

            Context.Items.Add("UID", MyUID);
            Context.Items.Add("Session", session);
            Context.Items.Add("Seat", playerSeat);
            await Groups.AddToGroupAsync(Context.ConnectionId, session.ToString());
            await Clients.Group(session.ToString()).SendAsync("AdminTriggerPing");
        }
    }
}
