using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace PJ_Source_GV.Hubs
{
    public class TicketHub: Hub
    {
        //Chỉ user trongg ticket đó nhận message 
        public async Task JoinTicket(int ticketId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, ticketId.ToString());
        }

        public async Task SendMessageToGroup(string ticketId)
        {
            await Clients.Group(ticketId).SendAsync("ReceiveMessage");
        }
    }
}
