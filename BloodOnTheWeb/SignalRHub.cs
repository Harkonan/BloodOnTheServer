using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace BloodOnTheWeb.Hubs
{
    
    public class Clocktower : Hub
    {
        public async Task ClientToServerVote(string voterId, string voterName, string newVote)
        {
            await Clients.All.SendAsync("ServerToClientVote", voterId, voterName, newVote);
        }

        public async Task ClientRequestsLatest()
        {
            await Clients.All.SendAsync("GetCurrentClientVote");
        }
    }
}
