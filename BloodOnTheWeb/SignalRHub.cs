using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace BloodOnTheWeb.Hubs
{
    
    public class Clocktower : Hub
    {
        public async Task ClientToServerVote(string voterId, string voterName, string newVote, string health, string vote_status, Guid session)
        {
            await Clients.Group(session.ToString()).SendAsync("ServerToClientVote", voterId, voterName, newVote, vote_status, health);
        }
            
        public async Task ClientRequestsLatest(Guid session)
        {
            await Clients.Group(session.ToString()).SendAsync("GetCurrentClientVote");
        }

        public async Task JoinSession(Guid session)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, session.ToString());
        }
    }
}
